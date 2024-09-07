using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

//厨房物品管理
public class KitchenGameManager : NetworkBehaviour
{
    public static KitchenGameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnGameLocalPaused;
    public event EventHandler OnGameLocalUnpaused;
    public event EventHandler OnMultiplayerGamePaused;
    public event EventHandler OnMultiplayerGameUnPaused;
    public event EventHandler OnLocalPlayerReadyChanged;

    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    [SerializeField] private Transform playerTransform;

    private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
    private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);
    private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0);
    private float gamePlayingTimerMax = 300f;
    private bool isLocalGamePaused = false;
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);
    private bool isLocalPlayerReady = false;
    private Dictionary<ulong, bool> playerReadyDictionary; //同步玩家
    private Dictionary<ulong, bool> playerPauseDictionary; //同步暂停
    private bool autoTestGamePauseState = false;

    private void Awake()
    {
        Instance = this;

        playerReadyDictionary = new Dictionary<ulong, bool>();
        playerPauseDictionary = new Dictionary<ulong, bool>();
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
    }

    private void LateUpdate()
    {
        if (autoTestGamePauseState)
        {
            autoTestGamePauseState = false;
            TestGamePauseState();
        }
    }

    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += State_OnValueChanged;
        isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;

        if (IsServer)
        {
            NetworkManager.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            NetworkManager.SceneManager.OnLoadEventCompleted += NetworkManager_OnLoadEventCompleted;
        }
    }

    private void NetworkManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach(ulong client in clientsCompleted)
        {
            Transform player = Instantiate(playerTransform);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(client, true);
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong obj)
    {
        autoTestGamePauseState = true;
    }

    private void State_OnValueChanged(State oldState, State newState)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void IsGamePaused_OnValueChanged(bool oldValue, bool newValue)
    {
        if (isGamePaused.Value)
        {
            Time.timeScale = 0;
            OnMultiplayerGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1;
            OnMultiplayerGameUnPaused?.Invoke(this, EventArgs.Empty);
        }
    }

    //游戏状态切换
    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (state.Value == State.WaitingToStart)
        {
            isLocalPlayerReady = true;
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
            SetPlayerReadyServerRpc();
        }
    }
    private void GameInput_OnPauseAction(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    [Rpc(SendTo.Server)]
    private void SetPlayerReadyServerRpc(RpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        //检查当前连接的客户端是否全部准备好了
        bool allClientReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                allClientReady = false;
                break;
            }
        }

        if (allClientReady)
        {
            state.Value = State.CountdownToStart;
        }
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        switch (state.Value)
        {
            case State.WaitingToStart:
                break;
            case State.CountdownToStart:
                countdownToStartTimer.Value -= Time.deltaTime;
                if (countdownToStartTimer.Value < 0f)
                {
                    state.Value = State.GamePlaying;
                    gamePlayingTimer.Value = gamePlayingTimerMax;
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer.Value -= Time.deltaTime;
                if (gamePlayingTimer.Value < 0f)
                {
                    state.Value = State.GameOver;
                }
                break;
            case State.GameOver:
                break;
        }
    }
    public bool IsGamePlaying()
    {
        return state.Value == State.GamePlaying;
    }

    public bool IsCountdownToStartActive()
    {
        return state.Value == State.CountdownToStart;
    }

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer.Value;
    }

    public bool IsGameOver()
    {
        return state.Value == State.GameOver;
    }

    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }

    public bool IsWaitingToStart()
    {
        return state.Value == State.WaitingToStart;
    }

    public float GetGamePlayingTimerNormalized()
    {
        return 1 - (gamePlayingTimer.Value / gamePlayingTimerMax);
    }

    public void TogglePauseGame()
    {
        isLocalGamePaused = !isLocalGamePaused;
        if (isLocalGamePaused)
        {
            PauseGameServerRpc();

            OnGameLocalPaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            UnPauseGameServerRpc();

            OnGameLocalUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    //暂停游戏
    [Rpc(SendTo.Server)]
    private void PauseGameServerRpc(RpcParams rpcParams = default)
    {
        playerPauseDictionary[rpcParams.Receive.SenderClientId] = true;
        TestGamePauseState();
    }

    //结束暂停
    [Rpc(SendTo.Server)]
    private void UnPauseGameServerRpc(RpcParams rpcParams = default)
    {
        playerPauseDictionary[rpcParams.Receive.SenderClientId] = false;
        TestGamePauseState();
    }

    //检查暂停
    private void TestGamePauseState()
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playerPauseDictionary.ContainsKey(clientId) && playerPauseDictionary[clientId])
            {                
                //暂停游戏
                isGamePaused.Value = true;
                return;
            }
        }
        //结束暂停
        isGamePaused.Value = false;
    }
}