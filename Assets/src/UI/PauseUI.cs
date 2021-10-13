using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseUI : MonoBehaviour
{
    public static PauseUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Transform MuteOffIcon;
    public Transform MusicOffIcon;

    public Transform Content;

    public void Activate()
    {
        var activated = !Content.gameObject.activeSelf;
        Content.gameObject.SetActive(activated);

        if (activated)
        {
            GameStateManager.Pause();
            MuteOffIcon.gameObject.SetActive(!Settings.VolumeToggle.Value);
            MusicOffIcon.gameObject.SetActive(!Settings.MusicToggle.Value);

            MusicController.LowerVolume();
        }
        else
        {
            GameStateManager.UnPause();

            MusicController.RaiseVolume();
        }
    }

    public void OnMuteClick()
    {
        Settings.VolumeToggle.Value = !Settings.VolumeToggle.Value;
        MuteOffIcon.gameObject.SetActive(!Settings.VolumeToggle.Value);
    }

    public void OnMusicClick()
    {
        Settings.MusicToggle.Value = !Settings.MusicToggle.Value;
        MusicOffIcon.gameObject.SetActive(!Settings.MusicToggle.Value);
    }

    public void OnLevelSelect()
    {
        GameSceneLoader.Instance.LoadLevelSelect();
    }

    public void OnRestart()
    {
        Content.gameObject.SetActive(false);
        GameStateManager.Retry();
    }

    public void OnResume()
    {
        Content.gameObject.SetActive(false);
        GameStateManager.UnPause();
    }
}
