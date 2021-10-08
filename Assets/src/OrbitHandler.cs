using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitHandler : MonoBehaviour
{
    public Vector2 StartingVelocity;

    public float StartingRotation;

    public int LineSegments = 60;

    private OrbitalRenderer orbitalRenderer;

    private new Rigidbody2D rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.velocity = StartingVelocity;
        rigidbody.angularVelocity = StartingRotation;

        orbitalRenderer = GetComponentInChildren<OrbitalRenderer>();
    }

    private void OnDrawGizmosSelected()
    {
        if (!this.isActiveAndEnabled) return;

        var orbit = GravitySystem.GetMyOrbit(transform, StartingVelocity);

        if (orbit == null) return;

        var points = orbit.GetLineSegments(LineSegments);

        var lastPoint = Vector3.zero;
        var hasLastPoint = false;
        foreach(var point in points)
        {
            if (!hasLastPoint)
            {
                lastPoint = point;
                hasLastPoint = true;
                continue;
            }


            Gizmos.DrawLine(lastPoint, point);
            lastPoint = point;
        }

        Gizmos.DrawLine(lastPoint, points[0]);
    }
    private void FixedUpdate()
    {
        var gforce = GravitySystem.GetMyGravityForce(transform);

        rigidbody.AddForce(gforce * Time.deltaTime, ForceMode2D.Impulse);

        var orbit = GravitySystem.GetMyOrbit(transform.transform, rigidbody.velocity);

        orbitalRenderer?.RenderOrbit(orbit);
    }
}
