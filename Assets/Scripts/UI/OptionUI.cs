using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    public static OptionUI Instance { get; private set; }
    [SerializeField] private Button soundEffectsButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI soundEffectsText;
    [SerializeField] private TextMeshProUGUI musicText;
    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI interactAltText;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button interactAltButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Transform pressToRebindKey;

    private void Awake()
    {
        Instance = this;
        soundEffectsButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.ChangeVolume();
            UpdataVisual();
        });
        musicButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ChangeVolume();
            UpdataVisual();
        });
        closeButton.onClick.AddListener(() =>
        {
            KitchenGameManager.Instance.TogglePauseGame();
            Hide();
        });

        moveUpButton.onClick.AddListener(() => { RedingBinding(GameInput.Binding.Move_Up); });
        moveDownButton.onClick.AddListener(() => { RedingBinding(GameInput.Binding.Move_Down); });
        moveLeftButton.onClick.AddListener(() => { RedingBinding(GameInput.Binding.Move_Left); });
        moveRightButton.onClick.AddListener(() => { RedingBinding(GameInput.Binding.Move_Right); });
        interactButton.onClick.AddListener(() => { RedingBinding(GameInput.Binding.Interact); });
        interactAltButton.onClick.AddListener(() => { RedingBinding(GameInput.Binding.InteractAlternate); });
        pauseButton.onClick.AddListener(() => { RedingBinding(GameInput.Binding.Pause); });
    }

    private void Start()
    {
        KitchenGameManager.Instance.OnGameUnPause += KitchenGameManager_OnGameUnPause;
        UpdataVisual();
        Hide();
    }

    private void KitchenGameManager_OnGameUnPause(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void UpdataVisual()
    {
        soundEffectsText.text = "Sound Effects:" + Mathf.Round(SoundManager.Instance.GetVolume() * 10f);
        musicText.text = "Music:" + Mathf.Round(MusicManager.Instance.GetVolume() * 10f);

        moveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
        moveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Down);
        moveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left);
        moveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right);
        interactText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        interactAltText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlternate);
        pauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ShowPressToRebindKey()
    {
        pressToRebindKey.gameObject.SetActive(true);
    }

    private void HidePressToRebindKey()
    {
        pressToRebindKey.gameObject.SetActive(false);
    }

    private void RedingBinding(GameInput.Binding binding)
    {
        ShowPressToRebindKey();
        GameInput.Instance.RebindBinding(binding, () =>
        {
            HidePressToRebindKey();
            UpdataVisual();
        });
    }
}