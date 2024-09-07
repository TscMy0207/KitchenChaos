using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameMutiplayer : NetworkBehaviour
{
    private const int MAX_PLAYER_AMOUNT = 4;//游戏最大连接数量

    public static KitchenGameMutiplayer Instance { get; private set; }

    [SerializeField] private KitchenObjectListSO kitchenObjectListSO;

    public event EventHandler OnFailToJoinGame;
    public event EventHandler OnTryToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;

    private NetworkList<PlayerData> playerDataNetworkList;
    [SerializeField] private List<Color> playerColorList = new List<Color>();

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);
        playerDataNetworkList = new NetworkList<PlayerData>();
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost()
    {
        NetworkManager.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback1;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_OnClientDisconnectCallback1(ulong obj)
    {
        for(int i = 0; i < playerDataNetworkList.Count; i++)
        {
            PlayerData playerData = playerDataNetworkList[i];
            if(playerData.clientID == obj)
            {
                playerDataNetworkList.RemoveAt(i);
            }
        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        playerDataNetworkList.Add(new PlayerData
        {
            clientID = clientId,
            colorId = GetFirstUnusedColorId()
        });
    }

    public void StartClient()
    {
        OnTryToJoinGame?.Invoke(this, EventArgs.Empty);
        NetworkManager.Singleton.StartClient();

        NetworkManager.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong obj)
    {
        OnFailToJoinGame?.Invoke(this, EventArgs.Empty);
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (NetworkManager.Singleton.ConnectedClientsIds.Count>= MAX_PLAYER_AMOUNT)
        {
            response.Approved = false;
            response.Reason = "游戏人数过多...";
            return;
        }
        if(SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString())
        {
            response.Approved = false;
            response.Reason = "游戏已经开始...";
            return;
        }

        //批准连接
        response.Approved = true;
    }

    //服务器无法序列自定义的参数 
    //所以通过ScriptableObject将参数存进List中 服务器就通过调用的方法来获取需要的数据
    //这样就避免了无法序列的数据经过服务器
    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
    }

    //NetworkObjectReference 网络对象参考 
    //将带有NetworkObject类的网络对象进行网络序列化 这样参数可以通过服务器
    //通过NetworkObjectReference.TryGet获取其中的NetworkObject
    //在通过NetworkObject就可以获取对应的组件
    [Rpc(SendTo.Server)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        //获得KitchenObjectSO
        KitchenObjectSO kitchenObjectSO = GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

        //网络对象生成物体
        //网络对象生成的物体会自动广播到客户机
        NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);

        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();

        //提取其中的组件
        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    }

    public int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO)
    {
        return kitchenObjectListSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);
    }

    public KitchenObjectSO GetKitchenObjectSOFromIndex(int kitchenObjectSOIndex)
    {
        return kitchenObjectListSO.kitchenObjectSOList[kitchenObjectSOIndex];
    }

    public void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }

    //删除物体
    [Rpc(SendTo.Server)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

        ClearKitchenObjectOnParentClientRpc(kitchenObjectNetworkObjectReference);
        kitchenObject.DestroySelf();
    }

    //清除父组件
    [Rpc(SendTo.ClientsAndHost)]
    private void ClearKitchenObjectOnParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

        kitchenObject.ClearKitchenObjectOnParent();
    }

    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < playerDataNetworkList.Count;
    }

    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for(int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientID == clientId)
            {
                return i;
            }
        }
        return -1;
    }

    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach(PlayerData playerDaat in playerDataNetworkList)
        {
            if(playerDaat.clientID == clientId)
            {
                return playerDaat;
            }
        }
        return default;
    }

    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkList[playerIndex];
    }

    public Color GetPlayerColorFromIndex(int index)
    {
        return playerColorList[index];
    }

    public void ChangePlayerColor(int colorId)
    {
        ChangePlayerColorServerRpc(colorId);
    }

    [Rpc(SendTo.Server)]
    private void ChangePlayerColorServerRpc(int colorId,RpcParams serverRpcParams = default)
    {
        if (!IsColorAvailable(colorId))
        {
            return;
        }

        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];
        playerData.colorId = colorId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    private bool IsColorAvailable(int colorId)
    {
        foreach(PlayerData playerData in playerDataNetworkList)
        {
            if(playerData.colorId == colorId)
            {
                return false;
            }
        }
        return true;
    }

    private int GetFirstUnusedColorId()
    {
        for(int i = 0; i < playerColorList.Count; i++)
        {
            if (IsColorAvailable(i))
            {
                return i;
            }
        }
        return -1;
    }
}
