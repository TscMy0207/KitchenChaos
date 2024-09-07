using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenObject {
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs : EventArgs {
        public KitchenObjectSO kitchenObjectSO;
    }

    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;

    private List<KitchenObjectSO> kitchenObjectSOList;

    protected override void Awake() {
        base.Awake();
        kitchenObjectSOList = new List<KitchenObjectSO>();
    }

    //尝试添加物品
    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO) {
        //如果食谱里面没有
        if (!validKitchenObjectSOList.Contains(kitchenObjectSO)) {
            return false;
        }
        //如果餐盘里面有
        if (kitchenObjectSOList.Contains(kitchenObjectSO)) {
            return false;
        } else {
            AddIngredientClientRpc(KitchenGameMutiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO));
            return true;
        }
    }

    [Rpc(SendTo.Server)]
    private void AddIngredientServerRpc(int kitchenObjectSOIndex)
    {
        AddIngredientClientRpc(kitchenObjectSOIndex);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void AddIngredientClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMutiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

        kitchenObjectSOList.Add(kitchenObjectSO);

        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
        {
            kitchenObjectSO = kitchenObjectSO
        });
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList() {
        return kitchenObjectSOList;
    }
}