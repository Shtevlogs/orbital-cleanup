using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temporary : MonoBehaviour
{
    public float Lifespan = 2f;

    public AudioReference Audio;

    private void Start()
    {
        if (Audio != null)
        {
            Audio.Init();
        }
    }

    private void Update()
    {
        Lifespan -= Time.deltaTime;
        if(Lifespan <= 0)
        {
            Destroy(gameObject);
        }
    }
}
