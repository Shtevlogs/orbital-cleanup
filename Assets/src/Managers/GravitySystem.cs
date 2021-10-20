using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteAlways]
public class GravitySystem : MonoBehaviour
{
    public static readonly double G = .0000000000667408;

    private static GravitySystem Instance;

    private static List<Planet> planets = new List<Planet>();

    [SerializeField]
    private Transform planetList;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        Instance = this;
        planets.Clear();
    }

    public static void InitializePlanets()
    {
        if(planets.Count > 0 || Instance == null)
        {
            return;
        }

        foreach (Transform child in Instance.planetList)
        {
            var planet = child.GetComponent<Planet>();
            if (planet != null && !planets.Contains(planet))
            {
                planets.Add(planet);
            }
        }
    }

    public static Vector2 GetMyGravityForce(Transform me, Planet limitedTo = null)
    {
        InitializePlanets();

        var gravity = new Vector2();

        if(limitedTo == null)
        {
            foreach (var p in planets)
            {
                var distance = Vector2.Distance(p.transform.position, me.position);
                if (distance > p.AtmosphereRadius)
                {
                    gravity += ((Vector2)(p.transform.position - me.position)).normalized * p.Mass / (distance * distance);
                }
            }
        }
        else
        {
            var distance = Vector2.Distance(limitedTo.transform.position, me.position);
            if (distance > limitedTo.AtmosphereRadius)
            {
                gravity = ((Vector2)(limitedTo.transform.position - me.position)).normalized * limitedTo.Mass / (distance * distance);
            }
        }

        return gravity;
    }

    public static Orbit GetMyOrbit(Transform me, Vector2 linearVelocity, Planet referenceBody = null)
    {
        InitializePlanets();

        var closestPlanet = referenceBody == null ? 
            planets.OrderBy(x => -x.Mass / Mathf.Pow(Vector2.Distance(me.position, x.transform.position),2f)).FirstOrDefault() : 
            referenceBody;

        if (closestPlanet == null) return null;

        var distance = (me.position - closestPlanet.transform.position).magnitude;

        return Orbit.FromPositionVelocity(closestPlanet.transform.position, me.position, linearVelocity, closestPlanet.Mass);
    }
}
