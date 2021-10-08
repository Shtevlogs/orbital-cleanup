using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public virtual void PickupAction( PlayerController player )
    {
        Destroy(gameObject);
    }

    public virtual void Destroy()
    {
        Destroy(gameObject);
    }
}
