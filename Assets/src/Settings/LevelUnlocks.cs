using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;

public class LevelUnlocks : MonoBehaviour
{
    private static LevelUnlocks Instance;

    private void Awake()
    {
        var isFirst = Instance == null;

        if (!isFirst) {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        Instance = this;
    }

    public List<LevelUnlockDefinition> UnlockDefs;

    public List<LevelDefinition> EarthLevels = new List<LevelDefinition>();
    public List<LevelDefinition> MarsLevels = new List<LevelDefinition>();
    public List<LevelDefinition> GloopLevels = new List<LevelDefinition>();

    public static LevelDefinition GetLevel(LevelLocation location)
    {
        var list = GetLevelList(location.Category);

        return list.Count > location.Index ? list[location.Index] : null;
    }

    public static List<LevelDefinition> GetLevelList(LevelCategory category)
    {
        return category == LevelCategory.Earth ? Instance.EarthLevels : (category == LevelCategory.Mars ? Instance.MarsLevels : Instance.GloopLevels); ;
    }

    public static bool GetUnlockStatus(LevelLocation location)
    {
        var levelDef = GetLevel(location);
        if (levelDef == null) return false;

        var relaventDef = Instance.UnlockDefs.Where(x => x.Unlocks.Contains(location)).FirstOrDefault();

        return _checkDef(relaventDef, location);
    }

    private static bool _checkDef(LevelUnlockDefinition unlockDef, LevelLocation location)
    {
        if (unlockDef == null) return true;

        var completedCount = 0;
        foreach(var preReqLocation in unlockDef.Prereqs)
        {
            if (!SaveData.Load(preReqLocation).Completed)
                continue;

            completedCount++;
            if(completedCount >= unlockDef.NumberOfPrereqsRequired)
            {
                return true;
            }
        }

        return false;
    }
}
