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
        VolumeToggle.Value = Extensions.GetBool(volumeToggle_key);
        VolumeToggle.OnSettingChanged += (value) => {
            Extensions.SetBool(volumeToggle_key, value);
            PlayerPrefs.Save();
        };

        MusicToggle.Value = Extensions.GetBool(musicToggle_key);
        MusicToggle.OnSettingChanged += (value) => {
            Extensions.SetBool(musicToggle_key, value);
            PlayerPrefs.Save();
        };
    }
}
