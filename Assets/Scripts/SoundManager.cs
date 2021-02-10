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

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
    }

    // Play a single clip through the sound effects source.
    public void Play(GameObject soundPlayerPrefab, Vector3 pos)
    {
        var inst = Instantiate(soundPlayerPrefab, pos, Quaternion.identity).GetComponent<AudioSource>();
        activeSources.Add(inst);
    }

    void Update()
    {
        foreach (var source in activeSources.Where(x => !x.isPlaying).ToList())
        {
            Destroy(source.gameObject);
            activeSources.Remove(source);
        }
    }
}
