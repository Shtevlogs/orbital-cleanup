using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuLogic : MonoBehaviour
{
    public void OnPlay()
    {
        Debug.Log("Hello");
        GameSceneLoader.Instance.LoadLevelSelect();
    }
}
