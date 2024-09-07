using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionUI : MonoBehaviour
{
    private void Start()
    {
        KitchenGameMutiplayer.Instance.OnTryToJoinGame += KitchenGameMutiplayer_OnTryToJoinGame;
        KitchenGameMutiplayer.Instance.OnFailToJoinGame += KitchenGameMutiplayer_OnFailToJoinGame;

        Hide();
    }

    private void KitchenGameMutiplayer_OnFailToJoinGame(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void KitchenGameMutiplayer_OnTryToJoinGame(object sender, System.EventArgs e)
    {
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        KitchenGameMutiplayer.Instance.OnTryToJoinGame -= KitchenGameMutiplayer_OnTryToJoinGame;
        KitchenGameMutiplayer.Instance.OnFailToJoinGame -= KitchenGameMutiplayer_OnFailToJoinGame;
    }
}
