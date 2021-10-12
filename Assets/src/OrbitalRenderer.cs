using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalRenderer : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public int LineSegments = 8;

    private void Start()
    {
        lineRenderer = lineRenderer == null ? GetComponent<LineRenderer>() : lineRenderer;
    }

    public void RenderOrbit(Orbit orbit)
    {
        lineRenderer = lineRenderer == null ? GetComponent<LineRenderer>() : lineRenderer;

        if (orbit == null)
        {
            lineRenderer.positionCount = 0;
            lineRenderer.SetPositions(new Vector3[0]);
            return;
        }

        var points = orbit.GetLineSegments(LineSegments);

        lineRenderer.positionCount = points.Length;

        lineRenderer.SetPositions(points);
    }
}
