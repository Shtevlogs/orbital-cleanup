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

    public Planet OrbitalReference;

    private Orbit orbit;

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

        var orbit = GravitySystem.GetMyOrbit(transform, StartingVelocity, OrbitalReference);

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
        var gforce = GravitySystem.GetMyGravityForce(transform, OrbitalReference);

        rigidbody.velocity += gforce * Time.fixedDeltaTime;

        orbit = GravitySystem.GetMyOrbit(transform.transform, rigidbody.velocity, OrbitalReference);

        orbitalRenderer?.RenderOrbit(orbit);
    }
}
