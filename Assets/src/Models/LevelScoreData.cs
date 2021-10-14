using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

[Serializable]
public class LevelScoreData
{
    public bool Completed = false;

    public int HighScore = -1;
    public int BestTime = -1;
}
