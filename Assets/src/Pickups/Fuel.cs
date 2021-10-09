using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fuel : Pickup
{
    public float Value = 1f;
    public override void PickupAction(PlayerController player)
    {
        player.FuelLevel = Mathf.Clamp01(player.FuelLevel + Value);

        base.PickupAction(player);
    }
}
