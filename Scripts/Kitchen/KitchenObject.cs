using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

//厨房物品
public class KitchenObject : NetworkBehaviour {
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private IKitchenObjectParent kitchenObjectParent;
    private FollowTransform followTransform;

    protected virtual void Awake()
    {
        followTransform = GetComponent<FollowTransform>();
    }

    public KitchenObjectSO GetKitchenObjectSO() {
        return kitchenObjectSO;
    }

    //设置物品父类
    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent) {
        SetKitchenObjectParentServerRpc(kitchenObjectParent.GetNetworkObject());
    }

    [Rpc(SendTo.Server)]
    private void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        SetKitchenObjectParentClientRpc(kitchenObjectParentNetworkObjectReference);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        //提取其中的组件
        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

        //有父类就清空父类
        if (this.kitchenObjectParent != null)
        {
            this.kitchenObjectParent.ClearKitchenObject();
        }
        //给物品添加父类
        this.kitchenObjectParent = kitchenObjectParent;

        //玩家手上有东西就报错
        if (kitchenObjectParent.HasKitchenObject())
        {
            Debug.LogError("IKitchenObjectParent already has a KitchenObject!");
        }
        //给玩家添加物品
        kitchenObjectParent.SetKitchenObject(this);

        //由于网络物体不能待在非网络对象的父类下
        //所以就用障眼法 时时设置KitchenObject的transform
        followTransform.SetTargetTransform(kitchenObjectParent.GetKitchenObjectFollowTransform());
    }

    public IKitchenObjectParent GetKitchenObjectParent() {
        return kitchenObjectParent;
    }

    //删除物体
    public void DestroySelf() {
        Destroy(gameObject);
    }

    //清空父组件
    public void ClearKitchenObjectOnParent()
    {
        kitchenObjectParent.ClearKitchenObject();
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject) {
        if (this is PlateKitchenObject) {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        } else {
            plateKitchenObject = null;
            return false;
        }
    }
    public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent) {
        KitchenGameMutiplayer.Instance.SpawnKitchenObject(kitchenObjectSO,kitchenObjectParent);
    }

    public static void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        KitchenGameMutiplayer.Instance.DestroyKitchenObject(kitchenObject);
    }
}