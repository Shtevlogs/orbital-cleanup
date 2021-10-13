using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashLogic : MonoBehaviour
{
    public AudioClip MenuMusic;

    private void Start()
    {
        var timer = GetComponent<Timer>();
        timer.OnTimerEnd += LoadScene;
        timer.StartTimer();

        MusicController.PlayTrack(MenuMusic);
    }

    public void LoadScene()
    {
        GameSceneLoader.Instance.LoadMenu();
    }
}
