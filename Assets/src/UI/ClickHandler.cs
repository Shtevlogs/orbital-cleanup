using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    private AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();     
    }

    public void Click() {
        if (Settings.VolumeToggle.Value)
        {
            source.Play();
        }
    }
}
