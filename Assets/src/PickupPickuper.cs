using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickupPickuper : MonoBehaviour
{
    public UnityEvent<Pickup> triggerEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        triggerEvent?.Invoke(collision.GetComponentInParent<Pickup>());
    }
}
