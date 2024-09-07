using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TrashCounter : BaseCounter {
    public static event EventHandler OnAnyObjectTrashed;

    new public static void ResetStaticData() {
        OnAnyObjectTrashed = null;
    }

    //�������ֻ��ͨ��������ɾ��
    //
    public override void Interact(Player player) {
        if (player.HasKitchenObject()) {
            KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
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
        OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
    }
}