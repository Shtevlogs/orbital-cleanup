using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicController : MonoBehaviour
{
    private static MusicController Instance;

    private void Awake()
    {
        var isFirst = Instance == null;

        if (!isFirst)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        Settings.MusicToggle.OnSettingChanged += _onMusicToggle;
        _onMusicToggle(Settings.MusicToggle.Value);

        Instance = this;
    }

    [SerializeField]
    private Crossfader AudioCrossfader;

    [SerializeField]
    private AudioMixer Mixer;
    [SerializeField]
    private AudioMixerSnapshot[] FilterSnapshots;

    private float _targetvolume = 1f;
    private float TargetVolume { get { return _targetvolume; } set { _targetvolume = Mathf.Clamp01(value); } }
    private float currentVolume;

    private void Start()
    {
        currentVolume = TargetVolume;
    }

    private bool musicJustToggled = false;

    private void _onMusicToggle(bool value)
    {
        AudioCrossfader.Toggle(value);

        if (value)
        {
            musicJustToggled = true;
        }
    }

    private void Update()
    {
        if (musicJustToggled)
        {
            AudioCrossfader.SetVolume(currentVolume);
            musicJustToggled = false;
        }

        if (currentVolume == TargetVolume) return;

        var direction = currentVolume > TargetVolume ? -1f : 1f;

        currentVolume = Mathf.Clamp01(currentVolume + direction * Time.unscaledDeltaTime);

        var newDirection = currentVolume > TargetVolume ? -1f : 1f;
        if(newDirection != direction)
        {
            currentVolume = TargetVolume;
        }

        AudioCrossfader.SetVolume(currentVolume);
    }

    public static void UpdatePlanetDistance(float dist)
    {
        if (Instance == null) return;

        dist = Mathf.Clamp01(dist);

        Instance.Mixer.TransitionToSnapshots(Instance.FilterSnapshots, new float[] { dist, 1f - dist }, Time.unscaledDeltaTime);
    }

    public static void PlayTrack(AudioClip track, float fadeTime = 1f)
    {
        if (Instance == null) return;

        if (Instance.AudioCrossfader.IsTrackPlaying(track)) return;

        var routine = Instance.AudioCrossfader.PlayTrack(track, fadeTime);
        if(routine != null)
            Instance.StartCoroutine(routine);
    }

    public static void LowerVolume()
    {
        if (Instance == null) return;
        Instance.TargetVolume = 0.5f;
    }

    public static void RaiseVolume()
    {
        if (Instance == null) return;
        Instance.TargetVolume = 1f;
    }
}
