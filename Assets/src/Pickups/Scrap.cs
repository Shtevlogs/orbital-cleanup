using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrap : Pickup
{
    public static int ScrapInLevel = 0;

    private void Start()
    {
        ScrapInLevel++;
    }

    public override void PickupAction(PlayerController player)
    {
        ScrapInLevel--;
        if(ScrapInLevel <= 0)
        {
            //end level logic
        }
        base.PickupAction(player);
    }

    public override void Destroy()
    {
        ScrapInLevel--;
        PlayerController.Instance.Damage();
        if (ScrapInLevel <= 0)
        {
            //end level logic
        }
        base.Destroy();
    }
}
