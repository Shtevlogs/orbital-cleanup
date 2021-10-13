using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    private static LoadingUI instance;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public Transform Bar;

    public void Activate()
    {
        gameObject.SetActive(true);
        Bar.localScale = new Vector3(0f, 1f, 1f);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void Set(float value)
    {
        value = Mathf.Clamp01(value);
        Bar.localScale = new Vector3(value, 1f, 1f);
    }
}
