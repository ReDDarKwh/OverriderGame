using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SoundManager : MonoBehaviour
{
    // Audio players components.

    // Singleton instance.
    public static SoundManager Instance = null;

    public HashSet<AudioSource> activeSources = new HashSet<AudioSource>();

    // Initialize the singleton instance.
    private void Awake()
    {
        // If there is not already an instance of SoundManager, set it to this.
        if (Instance == null)
        {
            Instance = this;
        }
        //If an instance already exists, destroy whatever this object is to enforce the singleton.
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Play a single clip through the sound effects source.
    public AudioSource Play(GameObject soundPlayerPrefab, Vector3 pos)
    {
        var inst = Instantiate(soundPlayerPrefab, pos, Quaternion.identity).GetComponent<AudioSource>();
        activeSources.Add(inst);

        return inst;
    }

    public void PauseAll()
    {
        foreach (var source in activeSources)
        {
            if (source != null)
            {
                source.Pause();
                source.tag = "Paused";
            }
        }
    }

    public void PlayAll()
    {
        foreach (var source in activeSources)
        {
            if (source != null)
            {
                source.Play();
                source.tag = "Untagged";
            }
        }
    }

    void Update()
    {
        foreach (var source in activeSources.Where(x => !x.isPlaying && x.tag != "Paused").ToList())
        {
            if (source != null)
            {
                Stop(source);
            }
        }
    }

    public void Stop(AudioSource source)
    {
        Destroy(source.gameObject);
        activeSources.Remove(source);
    }
}
