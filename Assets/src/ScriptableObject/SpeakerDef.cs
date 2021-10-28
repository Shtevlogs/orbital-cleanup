using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/Speaker", order = 1)]
public class SpeakerDef : ScriptableObject
{
    public string Name;
    public Sprite LipFlapSprite1;
    public Sprite LipFlapSprite2;
}
