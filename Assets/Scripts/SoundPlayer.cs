using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SoundPlayer : MonoBehaviour
{
    public SoundPreset[] soundPresets;
    private AudioSource[] sources;
    public void Play()
    {
        sources = soundPresets.Select(x => x.Play(transform.position)).ToArray();
    }

    public void Stop()
    {
        foreach (var source in sources)
        {
            SoundManager.Instance.Stop(source);
        }
    }

    public void PlayOne(int index)
    {
        SoundManager.Instance.Make(soundPresets[index], transform.position);
    }
}
