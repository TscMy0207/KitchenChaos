using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter {
    public event EventHandler OnPlayerGrabbedObject;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player) {
        if (!player.HasKitchenObject()) {
            // 如果玩家身上没有东西就把东西设置到玩家身上
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
            InteractLogicServerRpc();
        }
    }

    [Rpc(SendTo.Server)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InteractLogicClientRpc()
    {
        OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
    }
}