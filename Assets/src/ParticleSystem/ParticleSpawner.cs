using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    public Particle ToSpawn;
    public float SpawnRadius;
    public Vector2 SpawnDirectionLimit;

    public float Rate;
    public float RateVariation;

    public int SpawnLimit;

    public float MinLifespan;
    public float MaxLifespan;

    public bool fadeOverLife;
    public bool sproutAndShrinkOverLife;

    private bool spawnLimited;

    private float minSpawnTime;
    private float maxSpawnOffset;

    private float nextSpawn;

    private void Start()
    {
        var minRate = Mathf.Clamp(Rate - (RateVariation / 2f), 0.01f, float.MaxValue);
        var maxRate = Mathf.Clamp(Rate + (RateVariation / 2f), minRate, float.MaxValue);

        minSpawnTime = 1f / minRate;
        var maxSpawnTime = 1f / maxRate;
        maxSpawnOffset = maxSpawnTime - minSpawnTime;

        nextSpawn = _randomSpawnTime();

        spawnLimited = SpawnLimit > 0;
    }

    private void Update()
    {
        if (spawnLimited && SpawnLimit <= 0)
        {
            return;
        }

        if (Time.time > nextSpawn)
        {
            Vector2 randomPosition;

            if(SpawnDirectionLimit == Vector2.zero)
            {
                randomPosition = new Vector2(Random.value - 0.5f, Random.value - 0.5f).normalized * Random.value * SpawnRadius;
            }
            else
            {
                randomPosition = Quaternion.Euler(0f,0f, Vector2.Angle(Vector2.right, SpawnDirectionLimit)) * new Vector2(Random.value * 0.5f, Random.value - 0.5f).normalized * Random.value * SpawnRadius;
            }

            var randomRotation = Random.value * 360f - 180f;

            var particle = Instantiate(ToSpawn, transform.position + (Vector3)randomPosition, Quaternion.Euler(0, 0, randomRotation));
            particle.Timer = MinLifespan + Random.value * (MaxLifespan - MinLifespan);

            if (fadeOverLife)
                particle.Progress = _fadeParticle;
            if (sproutAndShrinkOverLife)
                particle.Progress += _sproutAndShrinkParticle;

            SpawnLimit -= 1;

            nextSpawn = _randomSpawnTime();
        }
    }

    private void _fadeParticle(float completion, GameObject go)
    {
        var sprite = go.GetComponentInChildren<SpriteRenderer>();
        if (sprite == null) return;

        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f - (completion * completion));
    }
    private void _sproutAndShrinkParticle(float completion, GameObject go)
    {
        var particle = go.transform.childCount > 0 ? go.transform.GetChild(0) : go.transform;

        if(completion < 0.1f)
        {
            completion = completion * 10f;

            particle.transform.localScale = (Vector3)(Vector2.one * completion) + Vector3.forward;
        }
        else
        {
            completion = (completion - 0.1f) * 1.11f;

            particle.transform.localScale = (Vector3)(Vector2.one * (1f - completion)) + Vector3.forward;
        }
    }

    private float _randomSpawnTime()
    {
        return Time.time + minSpawnTime + Random.value * maxSpawnOffset;
    }
}
