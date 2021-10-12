using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    private static Settings Instance;

    private void Awake()
    {
        var isFirst = Instance == null;

        if (!isFirst) {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        Instance = this;
        _pullSettings();
    }

    private static string volumeToggle_key = "volumeToggle";
    public static Setting<bool> VolumeToggle = new Setting<bool>();

    private static string musicToggle_key = "musicToggle";
    public static Setting<bool> MusicToggle = new Setting<bool>();

    private void _pullSettings()
    {
        VolumeToggle.Value = GetBool(volumeToggle_key);
        VolumeToggle.OnSettingChanged += (value) => { 
            SetBool(volumeToggle_key, value);
            PlayerPrefs.Save();
        };

        MusicToggle.Value = GetBool(musicToggle_key);
        MusicToggle.OnSettingChanged += (value) => {
            SetBool(musicToggle_key, value);
            PlayerPrefs.Save();
        };
    }

    private static bool GetBool(string key)
    {
        return PlayerPrefs.GetInt(key) == 1;
    }

    private static void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
    }
}
