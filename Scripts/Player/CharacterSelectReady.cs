using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance { get; private set; }

    private Dictionary<ulong, bool> playerReadyDictionary; //同步玩家

    public event EventHandler OnReadyChanged;

    private void Awake()
    {
        Instance = this;
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    public void PlayerReady()
    {
        SetPlayerReadyServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void SetPlayerReadyServerRpc(RpcParams serverRpcParams = default)
    {
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        //检查当前连接的客户端是否全部准备好了
        bool allClientReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                allClientReady = false;
                break;
            }
        }

        if (allClientReady)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(Loader.Scene.GameScene.ToString(),LoadSceneMode.Single);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetPlayerReadyClientRpc(ulong clientId)
    {
        playerReadyDictionary[clientId] = true;
        OnReadyChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsPlayerReady(ulong clientId)
    {
        return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId];
    }
}
