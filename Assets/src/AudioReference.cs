using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AudioReference
{
    public AudioSource AudioSource;
    public float MaxVolume;

    private bool volumeToggle = true;

    public void Init()
    {
        Settings.VolumeToggle.OnSettingChanged += _onVolumeToggleChanged;
        _onVolumeToggleChanged(Settings.VolumeToggle.Value);
    }

    public void Play(AudioClip clip = null)
    {
        if (!volumeToggle || AudioSource == null) return;

        if (clip != null)
            AudioSource.clip = clip;

        AudioSource.volume = MaxVolume;

        AudioSource.Play();
    }

    public void Toggle(bool play)
    {
        if (!volumeToggle || AudioSource == null) return;

        AudioSource.volume = MaxVolume;

        if (play && ! AudioSource.isPlaying)
        {
            AudioSource.Play();
        }

        if(!play && AudioSource.isPlaying)
        {
            AudioSource.Stop();
        }
    }

    public void Fade(float phase)
    {
        if (AudioSource != null)
            AudioSource.volume = volumeToggle ? MaxVolume * phase : 0f;
    }

    private void _onVolumeToggleChanged(bool newValue)
    {
        volumeToggle = newValue;
        if(newValue == false)
        {
            if(AudioSource != null)
                AudioSource.volume = 0f;
        }
    }
}
