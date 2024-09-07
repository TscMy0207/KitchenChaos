using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectUI : MonoBehaviour
{
    [SerializeField] private int colorIndex;
    [SerializeField] private Image image;
    [SerializeField] private GameObject selectGameObject;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            KitchenGameMutiplayer.Instance.ChangePlayerColor(colorIndex);
        });
    }

    private void Start()
    {
        KitchenGameMutiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMutiplayer_OnPlayerDataNetworkListChanged;
        image.color = KitchenGameMutiplayer.Instance.GetPlayerColorFromIndex(colorIndex);
        UpdateIsSelected();
    }

    private void KitchenGameMutiplayer_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        UpdateIsSelected();
    }

    private void UpdateIsSelected()
    {
        if(KitchenGameMutiplayer.Instance.GetPlayerData().colorId == colorIndex)
        {
            selectGameObject.SetActive(true);
        }
        else
        {
            selectGameObject.SetActive(false);
        }
    }
}
