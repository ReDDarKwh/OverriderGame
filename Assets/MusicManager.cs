using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    // Start is called before the first frame update
    public SoundPlayer player;
    void Start()
    {
        player.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
