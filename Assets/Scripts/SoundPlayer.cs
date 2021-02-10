using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;

    public void Play()
    {
        audioSource.PlayOneShot(audioClip);
    }
}
