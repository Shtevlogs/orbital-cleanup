using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[ExecuteInEditMode]
public class Planet : MonoBehaviour
{
    public float Mass;
    public float SurfaceRadius;
    public float AtmosphereRadius;
    public AnimatorController PlanetAnimator;
    public List<Planet> Moons;
    public List<PositionVelocity> MoonOrbits;

    public Color PlanetCloudColor = Color.white;

    private void OnValidate()
    {
        var colliderTransform = GetComponentInChildren<CircleCollider2D>().transform;
        var spriteTransform = transform.Find("PlanetSprite");
        var outlineTransform = transform.Find("PlanetOutline");
        var cloudTransform = transform.Find("PlanetCloud");
        var cloudRenderer = cloudTransform.GetComponent<SpriteRenderer>();
        var animator = GetComponent<Animator>();

        var scale = new Vector3(SurfaceRadius * 2f, SurfaceRadius * 2f, 1f);
        colliderTransform.localScale = scale;
        spriteTransform.localScale = scale;
        outlineTransform.localScale = scale;
        cloudTransform.localScale = scale;

        cloudRenderer.color = PlanetCloudColor;

        animator.runtimeAnimatorController = PlanetAnimator;

        for(var i = 0; i < Moons.Count; i++)
        {
            var moon = Moons[i];
            var orbit = MoonOrbits[i];

            var moonObject = Instantiate(moon, orbit.Position, Quaternion.identity, transform.parent);

            var orbitHandler = moonObject.GetComponent<OrbitHandler>();

            orbitHandler.StartingVelocity = orbit.Velocity;

            orbitHandler.enabled = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position, AtmosphereRadius);
    }
}
