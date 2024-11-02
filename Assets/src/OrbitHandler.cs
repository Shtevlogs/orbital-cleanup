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

    public bool verbose;

    private void Start()
    {
        ResetVelocity();
    }

    public void ResetVelocity()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        rigidbody.linearVelocity = StartingVelocity;
        rigidbody.angularVelocity = StartingRotation;

        orbitalRenderer = orbitalRenderer == null ? GetComponentInChildren<OrbitalRenderer>() : orbitalRenderer;

        if (orbitalRenderer == null) return;

        orbitalRenderer.RenderOrbit(GravitySystem.GetMyOrbit(rigidbody.transform, rigidbody.linearVelocity, OrbitalReference));
    }

    private void OnDrawGizmosSelected()
    {
        if (!this.isActiveAndEnabled) return;

        var orbit = GravitySystem.GetMyOrbit(transform, StartingVelocity, OrbitalReference);

        if (orbit == null) return;

        var points = orbit.GetLineSegments(LineSegments, transform.position);

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

        rigidbody.linearVelocity += gforce * Time.fixedDeltaTime;

        orbit = GravitySystem.GetMyOrbit(transform.transform, rigidbody.linearVelocity, OrbitalReference);

        if (verbose)
        {
            Debug.Log("Current Orbit: ");
            Debug.Log(orbit.eccentricity + ", " + orbit.orbitalangle + ", " + orbit.timeofperihelion + ", " + orbit.semimajoraxis + ", " + (orbit.clockwise ? "Clockwise" : "CCW"));
        }

        orbitalRenderer?.RenderOrbit(orbit);
    }
}
