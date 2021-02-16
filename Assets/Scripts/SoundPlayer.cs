using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public SoundPreset[] soundPresets;
    public void Play()
    {
        foreach (var soundPreset in soundPresets) SoundManager.Instance.Make(soundPreset, transform.position);
    }
}
