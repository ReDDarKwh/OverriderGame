using UnityEngine;

[CreateAssetMenu(fileName = "SoundPreset", menuName = "ScriptableObjects/SoundPreset")]
public class SoundPreset : ScriptableObject
{
    public AudioClip audioClip;
    public bool loop;
    public bool playOnAwake = true;
    [Range(-3, 3)]
    public float pitch = 1;
    [Range(0, 1)]
    public float volume = 1;
    [Range(0, 1)]
    public float spacialBlend;
    [Range(0, 256)]
    public int priority = 128;
    public bool ignorePause;


    public AudioSource Play(Vector3 pos)
    {
        return SoundManager.Instance.Make(this, pos);
    }
}
