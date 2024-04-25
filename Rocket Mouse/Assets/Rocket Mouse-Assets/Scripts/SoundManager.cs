using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public AudioSource bgm;
    public void Update()
    {
        PlayerPrefs.SetFloat("Volume",bgm.volume);
        PlayerPrefs.Save();
    }
}
