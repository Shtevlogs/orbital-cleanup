using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager Instance;

    public static bool IsPaused = true;

    [SerializeField]
    private LevelLoader levelLoader;

    [SerializeField]
    private Timer countdownTimer;

    [SerializeField]
    private Timer gameTimer;

    private void Awake()
    {
        Instance = this;
        countdownTimer.OnTimerEnd += _onCountdownEnd;
        gameTimer.OnTimerEnd += _onRoundEnd;

        Initialize();
    }

    private void Initialize()
    {
        _overviewMode(true);
        //start loading level
        levelLoader.BeginLevelLoad();

        //reset planets
        PlanetManager.ResetPlanets();

        //start countdown
        countdownTimer.gameObject.SetActive(true);
        countdownTimer.StartTimer();

        //reset round timer
        gameTimer.ResetTimer();
    }

    public static void Retry()
    {
        Instance.Initialize();
    }

    public static void Pause()
    {
        IsPaused = true;
        //pause physics
        Time.timeScale = 0f;
    }

    public static void UnPause()
    {
        IsPaused = false;
        //pause physics
        Time.timeScale = 1f;
    }

    private void _onCountdownEnd()
    {
        _overviewMode(false);
        gameTimer.StartTimer();
        countdownTimer.gameObject.SetActive(false);
    }

    private void _onRoundEnd()
    {
        _overviewMode(true);
        RoundEndUI.Instance.Activate(false, "Time Out!", 0, gameTimer.StartingTime, Scrap.ScrapInLevel);
    }

    private void _overviewMode(bool active)
    {
        if (active)
        {
            Pause();
            //position introduction camera
        }
        else
        {
            UnPause();
            Time.timeScale = 1f;
            //release intro camera
        }
    }
}
