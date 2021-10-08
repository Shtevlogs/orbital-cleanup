using System.Collections;
using UnityEngine;
using System;

public class Particle : MonoBehaviour
{
    public float Timer = 1f;

    public Action<float, GameObject> Progress;

    private float currentTime;

    private void Start()
    {
        currentTime = Timer;
    }

    private void Update()
    {
        currentTime = Mathf.Clamp(currentTime - Time.deltaTime, 0, float.MaxValue);

        Progress?.Invoke(1f - (currentTime / Timer), gameObject);

        if(currentTime <= 0)
        {
            Destroy(gameObject);
        }
    }
}