using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Crossfader
{
    public AudioSource Source1;
    public AudioSource Source2;

    public float Level = 1f;

    private int currentSource = 1;

    private bool fading = false;
    private IEnumerator nextFade;

    public bool IsPlaying()
    {
        return Source1.isPlaying || Source2.isPlaying;
    }

    public void SetVolume(float volume)
    {
        Level = volume;
        if (!fading)
        {
            _getSource(currentSource).volume = volume;
        }
    }

    public IEnumerator PlayTrack(AudioClip clip, float fadetime)
    {
        var fadeOutSource = _getSource(currentSource);
        var fadeInSource = _getSource(currentSource + 1);
        fadeInSource.clip = clip;
        fadeInSource.Play();

        currentSource = (currentSource + 1) % 2;

        if (!fading)
            return _fade(fadeOutSource, fadeInSource, fadetime);
        else
            nextFade = _fade(fadeOutSource, fadeInSource, fadetime);

        return null;
    }

    private IEnumerator _fade(AudioSource fadeOutSource, AudioSource fadeInSource, float time)
    {
        fading = true;

        yield return null;

        var startingTime = time;

        do
        {
            time = Mathf.Clamp(time - Time.unscaledDeltaTime, 0f, float.MaxValue);

            var progress = startingTime == 0f ? 1f : (startingTime - time) / startingTime;

            fadeOutSource.volume = Level * (1f - progress);
            fadeInSource.volume = Level * progress;

            yield return null;
        } while (time > 0f);

        fading = false;

        if (nextFade != null)
        {
            var temp = nextFade;
            nextFade = null;
            yield return temp;
        }
    }

    private AudioSource _getSource(int src)
    {
        src = src % 2;
        return src == 0 ? Source1 : Source2;
    }
}
