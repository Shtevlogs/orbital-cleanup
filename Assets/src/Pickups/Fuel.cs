using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fuel : Pickup
{
    public static int FuelInLevel = 0;

    public float Value = 1f;

    private void Start()
    {
        FuelInLevel++;
    }

    public override void PickupAction(PlayerController player)
    {
        FuelInLevel--;

        player.FuelLevel = Mathf.Clamp01(player.FuelLevel + Value);

        base.PickupAction(player);
    }
}
