using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{
    public AudioSource sound;
    private void Start()
    {
        float v = PlayerPrefs.GetFloat("Volume");
        sound.volume = v;
        
    }
}
