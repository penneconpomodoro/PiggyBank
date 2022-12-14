using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    public const float durationForReady = 5f;
    public const float durationForGame = 30f;
    public const float extendedDuration = 30f;
    public const float minGameAreaX = -3f;
    public const float maxGameAreaX = 3f;
    public const float minGameAreaY = 0f;
    public const float maxGameAreaY = 8f;

    public enum GameStatus
    {
        Ready,
        Active,
        GameOver
    }

    public int EarnedMoney { get; private set; }
    public float TimerToFinish { get; private set; }
    public int CurrentChain { get; private set; }
    public int MaxChain { get; private set; }
    public float ChainTimer { get; private set; }
    private readonly float resetChainTimer = 3f;
    public GameStatus gameStatus { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        InitGame();
    }

    private void InitGame()
    {
        EarnedMoney = 0;
        TimerToFinish = durationForReady;
        CurrentChain = 0;
        MaxChain = 0;
        ChainTimer = 0f;
        gameStatus = GameStatus.Ready;

        Debug.Log("Are you ready?");
    }

    private void StartGame()
    {
        EarnedMoney = 0;
        TimerToFinish = durationForGame;
        CurrentChain = 0;
        MaxChain = 0;
        ChainTimer = 0f;
        gameStatus = GameStatus.Active;
        PlayOneShotGameStart();

        Debug.Log("Game start!");
    }

    private void PlayOneShotGameStart()
    {
        SoundManager.instance.PlaySE(SoundManager.SeType.GameStart);
    }

    private void GameIsOver()
    {
        gameStatus = GameStatus.GameOver;
        PlayOneShotGameOver();

        PlayerDataManager.instance.numberOfPlays++;
        PlayerDataManager.instance.maxEarnedMoney = (int)Mathf.Max(PlayerDataManager.instance.maxEarnedMoney, EarnedMoney);
        PlayerDataManager.instance.totalEarnedMoney += EarnedMoney;
        PlayerDataManager.instance.maxChain = Mathf.Max(PlayerDataManager.instance.maxChain, MaxChain);
        PlayerDataManager.instance.Save();

        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(EarnedMoney, 0);
        //naichilab.RankingLoader.Instance.SendScoreAndShowRanking(MaxChain, 1);

        Debug.Log("Game is over.");
    }

    private void PlayOneShotGameOver()
    {
        SoundManager.instance.PlaySE(SoundManager.SeType.GameOver);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStatus == GameStatus.Ready)
        {
            TimerToFinish -= Time.deltaTime;
            TimerToFinish = Mathf.Max(0f, TimerToFinish);
        }
        else if (gameStatus == GameStatus.Active)
        {
            ChainTimer -= Time.deltaTime;
            ChainTimer = Mathf.Max(0, ChainTimer);
            TimerToFinish -= Time.deltaTime;
            TimerToFinish = Mathf.Max(0f, TimerToFinish);
        }
    }

    private void LateUpdate()
    {
        if (gameStatus == GameStatus.Ready)
        {
            if (TimerToFinish <= 0f)
            {
                StartGame();
            }
        }
        else if (gameStatus == GameStatus.Active)
        {
            if (ChainTimer <= 0f)
            {
                CurrentChain = 0;
            }
            if (TimerToFinish <= 0f)
            {
                GameIsOver();
            }
        }
    }

    private void CheckEarnedMoneyToExtendTimer(long old)
    {
        int[] thd = new int[] { 100, 1000, 10000, 100000, 1000000 };
        foreach (int i in thd)
        {
            if (old < i && EarnedMoney >= i)
            {
                ExtendTimer();
            }
        }
    }

    private void ExtendTimer()
    {
        TimerToFinish += extendedDuration;
    }

    public void EarnMoney(int money)
    {
        CurrentChain++;
        MaxChain = Mathf.Max(MaxChain, CurrentChain);
        ResetChainTimer();
        long oldEarnedMoney = EarnedMoney;
        EarnedMoney += money * CurrentChain;
        CheckEarnedMoneyToExtendTimer(oldEarnedMoney);
        PlayOneShotEarnMoney();
    }

    private void PlayOneShotEarnMoney()
    {
        float pitch = Mathf.Pow(2f, Mathf.Lerp(-1f, 1f, ((float)CurrentChain - 1f) / 7f));
        SoundManager.instance.PlaySE(SoundManager.SeType.EarnMoney, pitch);
    }

    private void ResetChainTimer()
    {
        ChainTimer = resetChainTimer;
    }

    public void AnyCoinChaging()
    {
        ResetChainTimer();
    }
}