using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

[Serializable]
public struct LevelLocation
{
    public LevelCategory Category;
    public int Index;

    public override string ToString()
    {
        return Category.ToString() + "." + Index.ToString();
    }
}
