using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    private static PlanetManager Instance;

    private static List<PositionVelocity> planetSpawns = new List<PositionVelocity>();

    private void Awake()
    {
        Instance = this;

        planetSpawns.Clear();

        foreach (Transform planet in transform) {
            planetSpawns.Add(new PositionVelocity { Position = planet.position, Velocity = planet.GetComponent<OrbitHandler>().StartingVelocity });
        }
    }

    public static void ResetPlanets()
    {
        if (Instance == null) return;

        for(var i = 0; i < Instance.transform.childCount; i++)
        {
            var planet = Instance.transform.GetChild(i);
            var spawn = planetSpawns[i];

            planet.position = i == 0 ? Vector3.zero : (Vector3)spawn.Position;
            if(i != 0)
                planet.GetComponent<OrbitHandler>().ResetVelocity();
        }
    }
}
