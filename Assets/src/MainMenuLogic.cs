using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuLogic : MonoBehaviour
{
    public void OnPlay()
    {
        GameSceneLoader.Instance.LoadLevelSelect();
    }
}
