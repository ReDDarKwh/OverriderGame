using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class SoundManager : MonoBehaviour
{
    // Audio players components.

    // Singleton instance.
    public static SoundManager Instance = null;
    public GameObject soundPrefab;
    public int poolSize;
    public Transform poolParent;
    public float fadeSpeed;
    private HashSet<AudioSource> activeSources = new HashSet<AudioSource>();
    private Stack<AudioSource> pool = new Stack<AudioSource>();
    private HashSet<AudioSource> fadeOutList = new HashSet<AudioSource>();

    // Initialize the singleton instance.
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitPool();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitPool()
    {
        for (var i = 0; i < poolSize; i++)
        {
            var inst = Instantiate(soundPrefab, poolParent);
            inst.SetActive(false);
            pool.Push(inst.GetComponent<AudioSource>());
        }
    }

    private AudioSource GetAudioSource(SoundPreset settings)
    {
        var audioSource = pool.Pop();
        audioSource.gameObject.SetActive(true);
        activeSources.Add(audioSource);

        audioSource.clip = settings.audioClip;
        audioSource.loop = settings.loop;
        audioSource.spatialBlend = settings.spacialBlend;
        audioSource.pitch = settings.pitch;
        audioSource.priority = settings.priority;
        audioSource.volume = settings.volume;
        audioSource.ignoreListenerPause = settings.ignorePause;

        return audioSource;
    }

    private void RemoveAudioSource(AudioSource audioSource)
    {
        pool.Push(audioSource);
        activeSources.Remove(audioSource);
        audioSource.gameObject.SetActive(false);
    }

    // Play a single clip through the sound effects source.
    public AudioSource Make(SoundPreset soundPreset, Vector3 pos)
    {
        var inst = GetAudioSource(soundPreset);
        inst.transform.position = pos;
        if (soundPreset.playOnAwake)
        {
            Play(inst);
        }
        return inst;
    }

    public void Play(AudioSource audioSource)
    {
        audioSource.Play();
        audioSource.tag = "Untagged";
    }

    public void Pause(AudioSource audioSource)
    {
        audioSource.Pause();
        audioSource.tag = "Paused";
    }

    public void PauseAll()
    {
        foreach (var source in activeSources)
        {
            if (source != null && !source.ignoreListenerPause)
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
            if (source != null && source.tag == "Paused")
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

        foreach (var source in fadeOutList.ToList())
        {
            if (source == null)
            {
                fadeOutList.Remove(source);
                continue;
            }

            source.volume -= fadeSpeed * Time.unscaledDeltaTime;

            if (source.volume <= 0)
            {
                Stop(source);
                fadeOutList.Remove(source);
            }
        }
    }

    public void FadeOut(AudioSource source)
    {
        if (source)
        {
            fadeOutList.Add(source);
        }
    }

    public void FadeOut(IEnumerable<AudioSource> sources)
    {
        if (sources != null)
        {
            foreach (var source in sources)
            {
                FadeOut(source);
            }
        }
    }

    public void Stop(AudioSource source)
    {
        RemoveAudioSource(source);
    }
}
