using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsListener
{
    public static AdManager Instance;

    public static bool AdActive = false;

    private long lastAdTime;

    private static long adBreakDuration = System.TimeSpan.TicksPerMinute * 5;

    private void Awake()
    {
        Instance = this;

        var adTimeSetting = PlayerPrefs.GetString("LastAdTime");
        lastAdTime = string.IsNullOrEmpty(adTimeSetting) ? System.DateTime.UtcNow.Ticks : long.Parse(adTimeSetting);
    }

#if UNITY_IOS
    private string gameId = "4398390";
    private string interstitial = "Interstitial_iOS";
    private string rewarded = "Rewarded_iOS";
#elif UNITY_ANDROID
    private string gameId = "4398391";
    private string interstitial = "Interstitial_Android";
    private string rewarded = "Rewarded_Android";
#endif

    private void Start()
    {
        Advertisement.Initialize(gameId, true, false);
    }

    public void ShowAd()
    {
        var now = System.DateTime.UtcNow.Ticks;

        if ((lastAdTime + adBreakDuration) > now || !Advertisement.IsReady() || LevelLoader.CurrentLevelLocation.AdFree)
        {
            AdActive = false;
            return;
        }

        lastAdTime = now;
        AdActive = true;
        Advertisement.Show(interstitial);
        Advertisement.AddListener(this);
    }

    public void OnUnityAdsReady(string placementId)
    {}

    public void OnUnityAdsDidError(string message)
    {
        Debug.LogError("Ad threw error: " + message);
        AdActive = false;
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        Debug.Log("Ad Started");
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        Debug.Log("Ad Finished");
        AdActive = false;
    }
}
