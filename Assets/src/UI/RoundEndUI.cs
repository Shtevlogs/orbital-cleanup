using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoundEndUI : MonoBehaviour
{
    public static RoundEndUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Transform Content;

    public TextMeshProUGUI HeaderMessage;

    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI ScrapLostText;

    public Button NextLevelButton;

    public AudioClip SuccessClip;
    public AudioClip FailClip;
    public AudioReference SuccessFailSound;

    public Button nextLevelButton;

    private bool success;

    private LevelLocation nextLevelLocation;

    private void Start()
    {
        SuccessFailSound.Init();
    }

    public void Activate(bool success, string message, int score, int time, int scrapLost)
    {
        this.success = success;

        SuccessFailSound.Play(success ? SuccessClip : FailClip);

        NextLevelButton.interactable = success;
        HeaderMessage.text = message;

        ScoreText.text = "" + score;
        TimeText.text = "" + time;
        ScrapLostText.text = "" + scrapLost;

        nextLevelLocation.Category = LevelLoader.CurrentLevelLocation.Category;
        nextLevelLocation.Index = LevelLoader.CurrentLevelLocation.Index + 1;

        nextLevelButton.interactable = success && LevelUnlocks.GetUnlockStatus(nextLevelLocation);

        Content.gameObject.SetActive(true);
    }

    public void OnBack()
    {
        GameSceneLoader.Instance.LoadLevelSelect();
    }

    public void OnRetry()
    {
        GameStateManager.Retry();
        Content.gameObject.SetActive(false);
    }

    public void OnNext()
    {
        if (!success || !nextLevelButton.interactable) return;

        GameSceneLoader.Instance.LoadLevelScene(nextLevelLocation);
    }
}
