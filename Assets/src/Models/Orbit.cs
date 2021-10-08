using UnityEditor;
using UnityEngine;
using System;

[Serializable]
public class Orbit
{
    public Vector2 CenterPosition;

    [NonSerialized]
    public float semimajoraxis;
    [NonSerialized]
    public float eccentricity;
    [NonSerialized]
    public float orbitalangle;
    [NonSerialized]
    public float timeofperihelion;

    public Orbit() {}

    public Orbit(Vector2 centerPosition, float semimajoraxis, float eccentricity, float orbitalangle, float timeofperihelion)
    {
        this.semimajoraxis = semimajoraxis;
        this.eccentricity = eccentricity;
        this.orbitalangle = orbitalangle;
        this.timeofperihelion = timeofperihelion;

        if(float.IsNaN(semimajoraxis) || float.IsNaN(eccentricity) || float.IsNaN(orbitalangle))
        {
            Debug.Log("nananana");
        }

        this.CenterPosition = centerPosition;
    }

    public static Orbit FromPositionVelocity(Vector2 planetPosition, Vector2 position, Vector2 velocity, float planetMassRatio, Orbit orbitToUpdate = null)
    {
        position = position - planetPosition;

        //position = new Vector2(3f, 6f);
        //velocity = new Vector2(-0.2f, 0.4f);

        //Debug.Log("position:(" + position.x + ", " + position.y + ")");
        //Debug.Log("velocity:[" + velocity.x + ", " + velocity.y + "]");

        var radius = position.magnitude;
        //Debug.Log("r = " + radius);
        var positionAngle = Vector2.SignedAngle(Vector2.right, position);
        //Debug.Log("theta = " + positionAngle);

        var speed = velocity.magnitude;
        //Debug.Log("V = " + speed);

        var velocityAngle = Vector2.SignedAngle(Vector2.right, velocity);
        //Debug.Log("vangle = " + velocityAngle);

        var semimajoraxis = 1f / ((2f / radius) - (speed * speed / planetMassRatio));
        //Debug.Log("a = " + semimajoraxis);

        var period = Mathf.Pow(semimajoraxis, 3f / 2f);
        //Debug.Log("P = " + period);

        var angularMomentum = radius * speed * Mathf.Sin((velocityAngle - positionAngle) * Mathf.Deg2Rad);

        var eccentricity = Mathf.Sqrt(1f - (Mathf.Pow(angularMomentum, 2f) / (semimajoraxis * planetMassRatio)));
        //Debug.Log("e = " + eccentricity);

        var orbitalAngleEq = (semimajoraxis * (1f - Mathf.Pow(eccentricity, 2f)) - radius) / (radius * eccentricity);
        //Debug.Log("cos(0 - w) = " + orbitalAngleEq);

        var positionAngleMinusOrbitalAngle = Mathf.Acos(Mathf.Clamp(orbitalAngleEq, -1f, 1f)) * Mathf.Rad2Deg;
        var positionAngleMinusOrbitalAngle2 = 360f - positionAngleMinusOrbitalAngle;
        //Debug.Log("0 - w = " + positionAngleMinusOrbitalAngle + " or " + positionAngleMinusOrbitalAngle2);

        //determine which is correct with some calc
        var tangentVelocitySlope = velocity.y / velocity.x;

        //Debug.Log("Target Velocity Slope: " + tangentVelocitySlope);

        var le = eccentricity * (1 - Mathf.Pow(eccentricity, 2f)) * semimajoraxis; //i haven't the slightest why they simplified this to l

        var angularVelocitySlope = le * Mathf.Sin(positionAngleMinusOrbitalAngle * Mathf.Deg2Rad) /
            Mathf.Pow(1 + eccentricity * Mathf.Cos(positionAngleMinusOrbitalAngle * Mathf.Deg2Rad), 2f);
        var angularVelocitySlope2 = le * Mathf.Sin(positionAngleMinusOrbitalAngle2 * Mathf.Deg2Rad) /
            Mathf.Pow(1 + eccentricity * Mathf.Cos(positionAngleMinusOrbitalAngle2 * Mathf.Deg2Rad), 2f);

        var velocitySlopeToCompare = (Mathf.Tan(positionAngle * Mathf.Deg2Rad) * angularVelocitySlope + radius) /
            (angularVelocitySlope - radius * Mathf.Tan(positionAngle * Mathf.Deg2Rad));
        var velocitySlopeToCompare2 = (Mathf.Tan(positionAngle * Mathf.Deg2Rad) * angularVelocitySlope2 + radius) /
            (angularVelocitySlope2 - radius * Mathf.Tan(positionAngle * Mathf.Deg2Rad));

        //Debug.Log("Velocity Slope To Compare 1: " + velocitySlopeToCompare);
        //Debug.Log("Velocity Slope To Compare 2: " + velocitySlopeToCompare2);

        var slope1Diff = Mathf.Abs(tangentVelocitySlope - velocitySlopeToCompare);
        var slope2Diff = Mathf.Abs(tangentVelocitySlope - velocitySlopeToCompare2);

        //pick whichever is closer (this is the lowercase v in the example)
        positionAngleMinusOrbitalAngle = (slope1Diff < slope2Diff ? positionAngleMinusOrbitalAngle : positionAngleMinusOrbitalAngle2);

        var orbitalAngle = positionAngle - positionAngleMinusOrbitalAngle;

        //Debug.Log("w = " + orbitalAngle);

        var tangle = (positionAngle - orbitalAngle) * Mathf.Deg2Rad;
        var E = Mathf.Acos((eccentricity + Mathf.Cos(tangle)) / (1 + eccentricity * Mathf.Cos(tangle))) * Mathf.Rad2Deg;
        var E2 = 360f - E;

        //align to positive angles
        while (E < 0) E += 360f;
        while (E2 < 0) E2 += 360f;
        while (positionAngleMinusOrbitalAngle < 0) positionAngleMinusOrbitalAngle += 360f;

        E = E % 360f;
        E2 = E2 % 360f;
        positionAngleMinusOrbitalAngle = positionAngleMinusOrbitalAngle % 360f;

        if (positionAngleMinusOrbitalAngle < 180f && E > 180f)
        {
            E = E2;
        }
        else if (positionAngleMinusOrbitalAngle > 180f && E < 180f)
        {
            E = E2;
        }

        var M = ((E * Mathf.Deg2Rad) - eccentricity * Mathf.Sin(E * Mathf.Deg2Rad)) * Mathf.Rad2Deg;

        var T = -M * Mathf.Deg2Rad * period / (2 * Mathf.PI);

        if(orbitToUpdate != null)
        {
            orbitToUpdate.CenterPosition = planetPosition;
            orbitToUpdate.semimajoraxis = semimajoraxis;
            orbitToUpdate.eccentricity = eccentricity;
            orbitToUpdate.orbitalangle = orbitalAngle;
            orbitToUpdate.timeofperihelion = T;

            return orbitToUpdate;
        }
        else
        {
            return new Orbit(planetPosition, semimajoraxis, eccentricity, orbitalAngle, T);
        }
    }

    public Vector2 GetPositionAtAngle(float angle)
    {
        var unitDirection = (Vector2) (Quaternion.Euler(0, 0, angle) * Vector2.right);

        var radius = semimajoraxis * (1f - Mathf.Pow(eccentricity, 2f)) / (1f + eccentricity * Mathf.Cos((angle - orbitalangle) * Mathf.Deg2Rad));

        return radius * unitDirection;
    }

    public Vector3[] GetLineSegments(int lineSegments)
    {
        var points = new Vector3[lineSegments];

        for (var i = 0; i < lineSegments; i++)
        {
            var toAdd = CenterPosition + GetPositionAtAngle(i * 360f / (float)lineSegments);
            if (float.IsNaN(toAdd.magnitude))
            {
                points[i] = Vector3.zero;
            }
            else
            {
                points[i] = CenterPosition + GetPositionAtAngle(i * 360f / (float)lineSegments);
            }
        }

        return points;
    }
}