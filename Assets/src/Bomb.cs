using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField]
    private LayerMask playermask;

    [SerializeField]
    private LayerMask pickupmask;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var go = collision.rigidbody.gameObject;
        if((playermask >> go.layer) == 1)
        {
            //player hit by bomb logic
            Destroy(gameObject);
            PlayerController.Instance.Damage();
        }
        else if((pickupmask >> go.layer) == 1)
        {
            //pickup hit by bomb logic
            var pickup = go.GetComponent<Pickup>();
            pickup.Destroy();
            Destroy(gameObject);
        }
    }
}
