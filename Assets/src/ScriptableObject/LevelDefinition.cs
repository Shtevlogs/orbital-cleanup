using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[SerializeField]
[CreateAssetMenu(fileName = "0", menuName = "ScriptableObjects/Level", order = 1)]
public class LevelDefinition : ScriptableObject
{
    public Scene LevelScene;

    public PositionVelocity PlayerStart;

    public List<PositionVelocity> Scraps;
    public List<PositionVelocity> Bombs;
    public List<PositionVelocity> Fuels;

    public AudioClip Music;
}
