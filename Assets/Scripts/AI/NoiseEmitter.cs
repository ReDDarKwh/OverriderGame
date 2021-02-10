using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using System.Linq;


public class NoiseEmitter : MonoBehaviour
{
    public LayerMask layersThatCanHear;
    public string noiseHeardEvent = "NoiseHeard";
    public float noiseRadius = 10;
    public int maxTargets = 100;

    public void EmitNoise()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, noiseRadius, layersThatCanHear);

        foreach (var collider in colliders.Take(maxTargets))
        {
            CustomEvent.Trigger(collider.gameObject, noiseHeardEvent, this.gameObject);
        }
    }
}
