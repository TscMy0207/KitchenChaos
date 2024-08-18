using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenGameManager : MonoBehaviour
{
    public static KitchenGameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePause;
    public event EventHandler OnGameUnPause;

    private enum State
    {
        WaitingToStart,
        CountDownToStart,
        GamePlayeing,
        GameOver,
    }

    private State state;
    private float waitingToStartTimer = 1f;
    private float countdownToStartTimer = 3f;
    private float gamePlayingTimer;
    private float gamePlayingTimerMax = 10f;
    private bool isGamePaused;

    private void Awake()
    {
        Instance = this;
        state = State.WaitingToStart;
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += KitchenGameManager_OnPauseAction;
    }

    private void KitchenGameManager_OnPauseAction(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    private void Update()
    {
        switch (state)
        {
            case State.WaitingToStart:
                waitingToStartTimer -= Time.deltaTime;
                if (waitingToStartTimer < 0f)
                {
                    state = State.CountDownToStart;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;

            case State.CountDownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer < 0f)
                {
                    state = State.GamePlayeing;
                    gamePlayingTimer = gamePlayingTimerMax;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;

            case State.GamePlayeing:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer < 0f)
                {
                    state = State.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;

            case State.GameOver:
                break;
        }
    }

    public bool IsGamePlaying()
    {
        return state == State.GamePlayeing;
    }

    public bool IsCountdownToStartActive()
    {
        return state == State.CountDownToStart;
    }

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer;
    }

    public bool IsGameOver()
    {
        return state == State.GameOver;
    }

    public float GetGamePlayingTimerNormalized()
    {
        return 1- (gamePlayingTimer / gamePlayingTimerMax);
    }

    public void TogglePauseGame()
    {
        isGamePaused = !isGamePaused;
        if (isGamePaused)
        {
            Time.timeScale = 0;
            OnGamePause?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            OnGameUnPause?.Invoke(this, EventArgs.Empty);
        }

    }
}