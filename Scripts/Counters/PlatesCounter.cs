using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter {
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;

    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int platesSpawnedAmount;
    private int platesSpawnedAmountMax = 4;

    private void Update() {
        if (!IsServer) return;

        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer > spawnPlateTimerMax) {
            spawnPlateTimer = 0f;

            if (KitchenGameManager.Instance.IsGamePlaying() && platesSpawnedAmount < platesSpawnedAmountMax) {
                SpawnPlateServerRpc();
            }
        }
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            if (platesSpawnedAmount > 0)
            {
                InteractLogicServerRpc();
                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
            }
        }
    }

    [Rpc(SendTo.Server)]
    private void SpawnPlateServerRpc()
    {
        SpawnPlateClientRpc();
    }

    //盘子数量增加
    [Rpc(SendTo.ClientsAndHost)]
    private void SpawnPlateClientRpc()
    {
        platesSpawnedAmount++;

        OnPlateSpawned?.Invoke(this, EventArgs.Empty);
    }

    [Rpc(SendTo.Server)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InteractLogicClientRpc()
    {
        // 生成盘子
        platesSpawnedAmount--;
        OnPlateRemoved?.Invoke(this, EventArgs.Empty);
    }
}