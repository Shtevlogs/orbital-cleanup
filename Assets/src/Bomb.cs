using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField]
    private LayerMask playermask;

    [SerializeField]
    private LayerMask pickupmask;

    [SerializeField]
    private Transform explosionPrefab;

    [SerializeField]
    private float explosionRadius = 1f;

    [SerializeField]
    public float explosionForce = 10f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var go = collision.rigidbody.gameObject;
        if((playermask >> go.layer) == 1)
        {
            //player hit by bomb logic
            PlayerController.Instance.Damage();

            explode();
        }
        else if((pickupmask >> go.layer) == 1)
        {
            //pickup hit by bomb logic
            var pickup = go.GetComponent<Pickup>();
            pickup.Destroy();

            explode();
        }
    }

    private void explode()
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity, null);
        var colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach(var col in colliders)
        {
            if(col.attachedRigidbody != null)
            {
                var line = col.attachedRigidbody.transform.position - transform.position;
                var dir = line.normalized;
                var dist = line.magnitude;

                col.attachedRigidbody.AddForceAtPosition(dir * explosionForce * (1f - (dist / explosionRadius) + 0.1f), transform.position, ForceMode2D.Impulse);
            }
        }

        Destroy(gameObject);
    }
}
