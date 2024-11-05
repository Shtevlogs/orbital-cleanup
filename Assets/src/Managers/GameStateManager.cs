using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager Instance;

    public static bool IsPaused = false;
    public static bool RoundStarting = true;
    public static bool IsGameplay = false;

    [SerializeField]
    private LevelLoader levelLoader;

    [SerializeField]
    private Timer countdownTimer;

    [SerializeField]
    private Timer gameTimer;

    private float roundStartTime;
    private bool roundEnded = false;

    private void Awake()
    {
        Instance = this;
        countdownTimer.OnTimerEnd += _onCountdownEnd;
        gameTimer.OnTimerEnd += _onRoundEnd;
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _initialize();
    }

    private void _initialize()
    {
        roundEnded = false;

        _overviewMode(true);
        //start loading level
        levelLoader.BeginLevelLoad();

        //reset planets
        PlanetManager.ResetPlanets();

        DalogueUI.OnDialogueFinish += _onDialogueFinished;

        //start dialogue
        var dialogue = levelLoader.WorkingLevel.Dialogue;
        if (dialogue.Phrases == null || dialogue.Phrases.Length == 0)
        {
            _onDialogueFinished();
        }
        else
        {
            DalogueUI.StartDialogue(dialogue.Phrases, dialogue.Speaker.LipFlapSprite1, dialogue.Speaker.LipFlapSprite2, dialogue.Speaker.name);
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
        else
        {
            IsPaused = false;
            //unpause physics
            Time.timeScale = 1f;
        }
    }

    private void _onDialogueFinished()
    {
        //only need this to fire once per binding
        DalogueUI.OnDialogueFinish -= _onDialogueFinished;

        //start countdown
        countdownTimer.gameObject.SetActive(true);
        countdownTimer.StartTimer();
        CameraBehaviour.Instance.ForceMaxZoom = true;

        if (levelLoader.WorkingLevel.Time > 0)
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

        var timePercent = 1f - (levelDef.Time <= 0 ? Mathf.Clamp01(0.25f + (totalTime / 240f)) : (totalTime / (float)Instance.gameTimer.StartingTime));
        var scrapsCollected = PlayerController.Instance.ScrapCollectedCount;
        var scrapsLost = levelDef.Scraps.Count - PlayerController.Instance.ScrapCollectedCount;
        var playerHealthPercent = (float)PlayerController.Instance.Health / (float) PlayerController.Instance.MaxHealth;

        var score = (int)Mathf.Clamp(1000f * timePercent + 100f * scrapsCollected + 300f * playerHealthPercent + 500f * Fuel.FuelInLevel + 500f + PlayerController.Instance.FuelLevel, 0f, float.MaxValue);

        Instance.gameTimer.StopTimer();

        //save data
        var currentLevelData = SaveData.Load(LevelLoader.CurrentLevelLocation);

        var newLevelsUnlocked = !currentLevelData.Completed && success && LevelUnlocks.WillUnlockLevels(LevelLoader.CurrentLevelLocation);

        currentLevelData.BestTime = currentLevelData.BestTime == -1 ? (int)totalTime : Mathf.Min(currentLevelData.BestTime, (int)totalTime);
        currentLevelData.HighScore = Mathf.Max(currentLevelData.HighScore, score);
        currentLevelData.Completed = currentLevelData.Completed || success;
        SaveData.Save(LevelLoader.CurrentLevelLocation, currentLevelData);
        SaveData.Persist();

        //gotta do this after saving, duh
        RoundEndUI.Instance.Activate(success, message, score, (int)totalTime, scrapsLost, newLevelsUnlocked);
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
