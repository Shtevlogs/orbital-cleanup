using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class LevelLoader : MonoBehaviour
{
    public static LevelDefinition NextLevel;
    public static LevelLocation CurrentLevelLocation;

    public LevelDefinition WorkingLevel;

    public bool SaveLevel = false;
    public bool LoadLevel = false;
    public bool ClearLevel = false;

    public Transform ScrapHolder;
    public Transform BombHolder;
    public Transform FuelHolder;
    public Transform AlienHolder;

    public PlayerController PlayerPrefab;
    public Scrap ScrapPrefab;
    public Bomb BombPrefab;
    public Fuel FuelPrefab;
    public Alien AlienPrefab;

    private void Update()
    {
        if (Application.isPlaying)
        {
            return;
        }

        if (SaveLevel)
        {
            _saveLevel();
            SaveLevel = false;
        }
        else if (LoadLevel)
        {
            _loadLevel();
            LoadLevel = false;
        }
        else if (ClearLevel)
        {
            _clearLevel();
            ClearLevel = false;
        }
    }

    private void _saveLevel()
    {
        if (Application.isPlaying)
            return;
        if(WorkingLevel == null)
        {
            Debug.LogError("please create a level and put it in the working level slot");
            return;
        }

        var spawnedScrapList = new List<PositionVelocity>();
        foreach(Transform scrapObject in ScrapHolder)
        {
            var orbit = scrapObject.GetComponent<OrbitHandler>();
            spawnedScrapList.Add(new PositionVelocity { Position = scrapObject.position, Velocity = orbit.StartingVelocity, Rotation = orbit.StartingRotation });
        }
        WorkingLevel.Scraps = spawnedScrapList;

        if(spawnedScrapList == null || spawnedScrapList.Count == 0)
        {
            Debug.LogError("please place at least one scrap");
            return;
        }

        var spawnedBombList = new List<PositionVelocity>();
        foreach (Transform bombObject in BombHolder)
        {
            var orbit = bombObject.GetComponent<OrbitHandler>();
            spawnedBombList.Add(new PositionVelocity { Position = bombObject.position, Velocity = orbit.StartingVelocity, Rotation = orbit.StartingRotation });
        }
        WorkingLevel.Bombs = spawnedBombList;

        var spawnedFuelList = new List<PositionVelocity>();
        foreach (Transform fuelObject in FuelHolder)
        {
            var orbit = fuelObject.GetComponent<OrbitHandler>();
            spawnedFuelList.Add(new PositionVelocity { Position = fuelObject.position, Velocity = orbit.StartingVelocity, Rotation = orbit.StartingRotation, FuelLevel = fuelObject.GetComponent<Fuel>().Value });
        }
        WorkingLevel.Fuels = spawnedFuelList;

        var spawnedAlienList = new List<PositionVelocity>();
        foreach(Transform alienObject in AlienHolder)
        {
            var orbit = alienObject.GetComponent<OrbitHandler>();
            spawnedAlienList.Add(new PositionVelocity { Position = alienObject.position, Velocity = orbit.StartingVelocity, Rotation = orbit.StartingRotation });
        }
        WorkingLevel.Aliens = spawnedAlienList;

        var player = FindFirstObjectByType<PlayerController>();

        if(player == null)
        {
            Debug.LogError("Please place a player");
            return;
        }

        var playerOrbitHandler = player.gameObject.GetComponent<OrbitHandler>();
        WorkingLevel.PlayerStart = new PositionVelocity { 
            Position = playerOrbitHandler.transform.position, 
            Velocity = playerOrbitHandler.StartingVelocity,
            Rotation = player.transform.rotation.eulerAngles.z
        };

        WorkingLevel.LevelScene = (GameSceneLoader.GameScene)SceneManager.GetActiveScene().buildIndex;

        var cameraBehaviour = FindFirstObjectByType<CameraBehaviour>();
        if (cameraBehaviour != null)
        {
            WorkingLevel.MaxCameraRange = cameraBehaviour.MaxBounds.x;
        }

#if UNITY_EDITOR
        EditorUtility.SetDirty(WorkingLevel);
#endif
    }

    public void BeginLevelLoad() 
    {
        _loadLevel();
    }

    private void _clearLevel()
    {
        var player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            _destroy(player.gameObject);
        }

        _destroyChildren(ScrapHolder);
        _destroyChildren(BombHolder);
        _destroyChildren(FuelHolder);
        _destroyChildren(AlienHolder);

        Scrap.ScrapInLevel = 0;

        var cameraBehaviour = FindFirstObjectByType<CameraBehaviour>();
        if(cameraBehaviour != null)
        {
            cameraBehaviour.MaxBounds = Vector2.one * 9f;
        }
    }

    private void _loadLevel()
    {
        _clearLevel();

        if(NextLevel != null)
        {
            WorkingLevel = NextLevel;
            NextLevel = null;
        }

        if (WorkingLevel == null)
        {
            Debug.LogError("please create a level and put it in the working level slot");
            return;
        }

        var player = Instantiate(PlayerPrefab, transform.parent);

        player.transform.position = WorkingLevel.PlayerStart.Position;
        player.transform.rotation = Quaternion.Euler(0,0,WorkingLevel.PlayerStart.Rotation);

        PlayerController.FullMusicRadius = PlanetManager.GetRootPlanetRadius();
        PlayerController.NoMusicRadius = WorkingLevel.MaxCameraRange;

        var playerOrbitHandler = player.GetComponent<OrbitHandler>();
        playerOrbitHandler.StartingVelocity = WorkingLevel.PlayerStart.Velocity;
        playerOrbitHandler.ResetVelocity();

        player.FuelLevel = Mathf.Clamp01(WorkingLevel.StartingFuel);
        player.Health = WorkingLevel.StartingHealth;
        player.MaxHealth = WorkingLevel.StartingHealth;

        foreach (var scrapPlacement in WorkingLevel.Scraps)
        {
            var newScrap = Instantiate(ScrapPrefab, ScrapHolder);
            var newScrapOrbit = newScrap.GetComponent<OrbitHandler>();
            newScrap.transform.position = scrapPlacement.Position;
            newScrapOrbit.StartingVelocity = scrapPlacement.Velocity;
            newScrapOrbit.StartingRotation = scrapPlacement.Rotation == 0 ? (Random.value * 180f - 90f) : scrapPlacement.Rotation;
            newScrapOrbit.ResetVelocity();
        }

        foreach (var bombPlacement in WorkingLevel.Bombs)
        {
            var newBomb = Instantiate(BombPrefab, BombHolder);
            var newBombOrbit = newBomb.GetComponent<OrbitHandler>();
            newBomb.transform.position = bombPlacement.Position;
            newBombOrbit.StartingVelocity = bombPlacement.Velocity;
            newBombOrbit.StartingRotation = bombPlacement.Rotation == 0 ? (Random.value * 180f - 90f) : bombPlacement.Rotation;
            newBombOrbit.ResetVelocity();
        }

        foreach (var fuelPlacement in WorkingLevel.Fuels)
        {
            var newFuel = Instantiate(FuelPrefab, FuelHolder);
            var newFuelOrbit = newFuel.GetComponent<OrbitHandler>();
            newFuel.transform.position = fuelPlacement.Position;
            newFuelOrbit.StartingVelocity = fuelPlacement.Velocity;
            newFuelOrbit.StartingRotation = fuelPlacement.Rotation == 0 ? (Random.value * 180f - 90f) : fuelPlacement.Rotation;
            newFuelOrbit.ResetVelocity();
            newFuel.Value = fuelPlacement.FuelLevel;
        }

        foreach(var alienPlacement in WorkingLevel.Aliens)
        {
            var newAlien = Instantiate(AlienPrefab, AlienHolder);
            var newAlienOrbit = newAlien.GetComponent<OrbitHandler>();
            newAlien.transform.position = alienPlacement.Position;
            newAlienOrbit.StartingVelocity = alienPlacement.Velocity;
            newAlienOrbit.StartingRotation = alienPlacement.Rotation;
            newAlienOrbit.ResetVelocity();
        }

        if(Application.isPlaying)
            MusicController.PlayTrack(WorkingLevel.Music);

        var cameraBehaviour = FindFirstObjectByType<CameraBehaviour>();
        if (cameraBehaviour != null)
        {
            cameraBehaviour.MaxBounds = Vector2.one * WorkingLevel.MaxCameraRange;
        }
    }

    private void _destroyChildren(Transform parent)
    {
        if (!Application.isPlaying)
        {
            while(parent.childCount > 0)
            {
                DestroyImmediate(parent.GetChild(0).gameObject);
            }
        }
        else
        {
            foreach(Transform child in parent)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }

    private void _destroy(GameObject go)
    {
        if (!Application.isPlaying)
        {
            DestroyImmediate(go);
        }
        else
        {
            GameObject.Destroy(go);
        }
    }
}
