using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionResponseMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private Button close;

    private void Start()
    {
        close.onClick.AddListener(() =>
        {
            Hide();
        });
        KitchenGameMutiplayer.Instance.OnFailToJoinGame += KitchenGameMutiplayer_OnFailToJoinGame;

        Hide();
    }

    private void KitchenGameMutiplayer_OnFailToJoinGame(object sender, System.EventArgs e)
    {
        Show();
        message.text = NetworkManager.Singleton.DisconnectReason;
        if(message.text == "")
        {
            message.text = "无法连接";
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

    private void OnDestroy()
    {
        KitchenGameMutiplayer.Instance.OnFailToJoinGame -= KitchenGameMutiplayer_OnFailToJoinGame;
    }
}
