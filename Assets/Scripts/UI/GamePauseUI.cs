using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button optionsButton;

    private void Awake()
    {
        resumeButton.onClick.AddListener(() =>
        {
            KitchenGameManager.Instance.TogglePauseGame();
        });
        mainMenuButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        optionsButton.onClick.AddListener(() =>
        {
            OptionUI.Instance.Show();
            Hide();
        });
    }

    private void Start()
    {
        KitchenGameManager.Instance.OnGamePause += KitchenGameMananger_OnGamePause;
        KitchenGameManager.Instance.OnGameUnPause += KitchenGameManager_OnGameUnPause;
        Hide();
    }

    private void KitchenGameManager_OnGameUnPause(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void KitchenGameMananger_OnGamePause(object sender, System.EventArgs e)
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
}