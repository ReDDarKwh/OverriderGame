using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmitNoiseAction : Action
{
    public NoiseEmitter noiseEmitter;
    public float emissionInterval;
    public SoundPlayer player;
    private float lastEmission;

    internal override void OnStart()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastEmission > emissionInterval)
        {
            if (outputGate.currentValue)
            {
                lastEmission = Time.time;
                if (player)
                {
                    player.Play();
                }
                noiseEmitter.EmitNoise();
            }
        }
    }
}
