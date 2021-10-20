using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager Instance;

    public static bool IsPaused = false;
    public static bool RoundStarting = true;

    [SerializeField]
    private LevelLoader levelLoader;

    [SerializeField]
    private Timer countdownTimer;

    [SerializeField]
    private Timer gameTimer;

    private float roundStartTime;
    private bool roundEnded = false;

    [SerializeField]
    private Transform testingInputManager;

    private void Awake()
    {
        Instance = this;
        countdownTimer.OnTimerEnd += _onCountdownEnd;
        gameTimer.OnTimerEnd += _onRoundEnd;

        if (SceneManager.sceneCount == 1)
        {
            testingInputManager.gameObject.SetActive(true);
        }
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        roundEnded = false;

        _overviewMode(true);
        //start loading level
        levelLoader.BeginLevelLoad();

        //reset planets
        PlanetManager.ResetPlanets();

        //start countdown
        countdownTimer.gameObject.SetActive(true);
        countdownTimer.StartTimer();
        CameraBehaviour.Instance.ForceMaxZoom = true;

        if(levelLoader.WorkingLevel.Time > 0)
        {
            //reset round timer
            gameTimer.StartingTime = levelLoader.WorkingLevel.Time;
            gameTimer.ResetTimer();
        }
        else
        {
            gameTimer.gameObject.SetActive(false);
        }
    }

    public static void Retry()
    {
        Instance.Initialize();
    }

    public static void Pause()
    {
        if (RoundStarting && Instance != null && Instance.countdownTimer != null)
            Instance.countdownTimer.StopTimer();

        IsPaused = true;
        //pause physics
        Time.timeScale = 0f;
    }

    public static void UnPause()
    {
        if (RoundStarting && Instance != null && Instance.countdownTimer != null)
            Instance.countdownTimer.ResumeTimer();

        IsPaused = false;
        //pause physics
        Time.timeScale = 1f;
    }

    private void _onCountdownEnd()
    {
        _overviewMode(false);
        if(gameTimer.gameObject.activeSelf) gameTimer.StartTimer();
        countdownTimer.gameObject.SetActive(false);
        roundStartTime = Time.time;
        CameraBehaviour.Instance.ForceMaxZoom = false;
    }

    private void _onRoundEnd()
    {
        _overviewMode(true);
        EndRound(false, "Time Out!");
    }

    public static void EndRound(bool success, string message)
    {
        if (Instance.roundEnded) return;

        Instance.roundEnded = true;

        var levelDef = Instance.levelLoader.WorkingLevel;

        var totalTime = Time.time - Instance.roundStartTime;

        var timePercent = 1f - (Instance.gameTimer.StartingTime == 0 ? (totalTime / 120f) : (totalTime / (float)Instance.gameTimer.StartingTime));
        var scrapsCollected = PlayerController.Instance.ScrapCollectedCount;
        var scrapsLost = levelDef.Scraps.Count - PlayerController.Instance.ScrapCollectedCount;
        var playerHealthPercent = (float)PlayerController.Instance.Health / 3f;

        var score = (int)Mathf.Clamp(1000f * timePercent + 100f * scrapsCollected - 200f * scrapsLost + 300f * playerHealthPercent, 0f, float.MaxValue);

        Instance.gameTimer.StopTimer();

        //save data
        var currentLevelData = SaveData.Load(LevelLoader.CurrentLevelLocation);
        currentLevelData.BestTime = currentLevelData.BestTime == -1 ? (int)totalTime : Mathf.Min(currentLevelData.BestTime, (int)totalTime);
        currentLevelData.HighScore = Mathf.Max(currentLevelData.HighScore, score);
        currentLevelData.Completed = currentLevelData.Completed || success;
        SaveData.Save(LevelLoader.CurrentLevelLocation, currentLevelData);
        SaveData.Persist();

        //gotta do this after saving, duh
        RoundEndUI.Instance.Activate(success, message, score, (int)totalTime, scrapsLost);
    }

    private void _overviewMode(bool active)
    {
        if (active)
        {
            Time.timeScale = 0f;
            //position introduction camera
        }
        else
        {
            Time.timeScale = 1f;
            //release intro camera
        }
        RoundStarting = active;
    }
}
