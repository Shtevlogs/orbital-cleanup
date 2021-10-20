using UnityEngine;
using System;

[Serializable]
public class PositionVelocity
{
    public Vector2 Position;
    public Vector2 Velocity;
    public float Rotation;

    public float FuelLevel = 0.5f;
}