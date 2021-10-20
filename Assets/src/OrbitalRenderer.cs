using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

        var points = orbit.GetLineSegments(LineSegments, transform.parent.position);

        if(orbit.semimajoraxis < 0 && points.Length >= 4)
        {
            for(int i = points.Length / 4; i < points.Length; i++)
            {
                points[i] = points[(points.Length/4) - 1];
            }
            lineRenderer.loop = false;
        }
        else
        {
            lineRenderer.loop = true;
        }

        lineRenderer.positionCount = points.Length;

        lineRenderer.SetPositions(points);
    }
}
