using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneLoader : MonoBehaviour
{
    public enum GameScene
    {
        Empty = 0,
        Splash = 1,
        MainMenu = 2,
        LevelSelect = 3,

        EarthTraining = 4,
        Earth = 5,
        EarthMoon = 6,

        Mars = 7,

        Gloop = 8,
        GloopParty = 9
    }

    public static GameSceneLoader Instance;

    [SerializeField]
    public LoadingUI loadingUI;

    private GameScene currentScene;
    private List<AsyncOperation> scenesLoading = new List<AsyncOperation>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(this);

        SceneManager.LoadSceneAsync((int)GameScene.Splash, LoadSceneMode.Additive);
        currentScene = GameScene.Splash;
    }

    public void LoadMenu()
    {
        if (currentScene == GameScene.MainMenu) return;

        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)currentScene));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)GameScene.MainMenu, LoadSceneMode.Additive));

        currentScene = GameScene.MainMenu;
    }

    public void LoadLevelSelect()
    {
        if (currentScene == GameScene.LevelSelect) return;

        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)currentScene));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)GameScene.LevelSelect, LoadSceneMode.Additive));

        currentScene = GameScene.LevelSelect;
    }

    public void LoadLevelScene(LevelLocation location)
    {
        var level = LevelUnlocks.GetLevel(location);

        if(level == null)
        {
            Debug.LogError("Asked To Load A Level That Doesn't Exist");
            return;
        }

        loadingUI.Activate();

        LevelLoader.NextLevel = level;
        LevelLoader.CurrentLevelLocation = location;

        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)currentScene));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)level.LevelScene, LoadSceneMode.Additive));

        currentScene = level.LevelScene;

        AdManager.Instance.ShowAd();

        StartCoroutine(_loadScenes());
    }

    private IEnumerator _loadScenes()
    {
        for(var i = 0; i < scenesLoading.Count; i++)
        {
            while (!scenesLoading[i].isDone)
            {
                var loadProgress = 0f;

                foreach(var operation in scenesLoading)
                {
                    loadProgress += operation.progress;
                }

                loadingUI.Set(loadProgress / (float)scenesLoading.Count);

                yield return null;
            }
        }

        while (AdManager.AdActive)
        {
            yield return null;
        }

        scenesLoading.Clear();

        loadingUI.Deactivate();
    }
}
