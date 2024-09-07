using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject ready;
    [SerializeField] private PlayerVisual playerVisual;

    private void Start()
    {
        KitchenGameMutiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMutiplayer_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;
        Hide();
        UpdatePlayer();
    }

    private void CharacterSelectReady_OnReadyChanged(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }

    private void KitchenGameMutiplayer_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (KitchenGameMutiplayer.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();

            PlayerData playerData = KitchenGameMutiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            ready.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientID));
            playerVisual.SetPlayerColor(KitchenGameMutiplayer.Instance.GetPlayerColorFromIndex(playerData.colorId));
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
