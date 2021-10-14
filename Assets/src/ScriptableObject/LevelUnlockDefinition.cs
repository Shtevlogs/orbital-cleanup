using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

[Serializable]
[CreateAssetMenu(fileName = "0", menuName = "ScriptableObjects/LevelUnlock", order = 1)]
public class LevelUnlockDefinition : ScriptableObject
{
    public int NumberOfPrereqsRequired = 1;
    public LevelLocation[] Prereqs;
    public LevelLocation[] Unlocks;
}
