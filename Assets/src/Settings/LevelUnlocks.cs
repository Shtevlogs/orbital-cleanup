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
        if (Instance == null) return new List<LevelDefinition>();

        return category == LevelCategory.Earth ? Instance.EarthLevels : (category == LevelCategory.Mars ? Instance.MarsLevels : Instance.GloopLevels); ;
    }

    public static bool GetUnlockStatus(LevelLocation location)
    {
        var levelDef = GetLevel(location);
        if (levelDef == null) return false;

        var relaventDef = Instance.UnlockDefs.Where(x => x.Unlocks.Contains(location)).FirstOrDefault();

        return _isComplete(_checkDef(relaventDef));
    }

    public static bool WillUnlockLevels(LevelLocation completedLevelLocation)
    {
        var relaventDefs = Instance.UnlockDefs.Where(x => x.Prereqs.Contains(completedLevelLocation));

        return relaventDefs.Any(x => {
            var progress = _checkDef(x);

            return (progress[1] - progress[0]) == 1; //would be unlocked by this level being completed
        });
    }

    public static int[] LevelUnlockCount(LevelCategory planet)
    {
        var relaventDefs = Instance.UnlockDefs.Where(x => x.Prereqs.Any(y=> y.Category == planet));

        var incompleteDefs = relaventDefs.Where(x => !_isComplete(_checkDef(x)));

        if(incompleteDefs == null || incompleteDefs.Count() == 0)
        {
            return new int[] { 1, 1 };
        }

        var mostCompleteDef = incompleteDefs.OrderBy(x => {
            var progress = _checkDef(x);
            return progress[1] - progress[0];
        }).First();

        return _checkDef(mostCompleteDef);
    }

    private static int[] _checkDef(LevelUnlockDefinition unlockDef)
    {
        if (unlockDef == null) return new int[] { 1, 1 };

        var completedCount = 0;
        foreach(var preReqLocation in unlockDef.Prereqs)
        {
            if (!SaveData.Load(preReqLocation).Completed)
                continue;

            completedCount++;
        }

        return new int[] { completedCount, unlockDef.NumberOfPrereqsRequired };
    }

    private static bool _isComplete(int[] progress)
    {
        return progress != null && progress[0] >= progress[1];
    }
}
