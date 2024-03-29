﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MoreLinq;

public class NoiseEmitter : MonoBehaviour
{
    public LayerMask layersThatCanHear;
    public NoiseEmitterEvents noiseHeardEvent = NoiseEmitterEvents.noiseHeard;
    public float noiseRadius = 10;
    public int maxTargets = 100;
    public ParticleSystem rippleEffect;
    public GameObject interactableObject;
    public Collider2D[] ignoredColliders;
    public LayerMask soundBlockingLayers;

    public enum NoiseEmitterEvents {
        noiseHeard,
        objectNoiseHeard
    }

    public void EmitNoise()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, noiseRadius, layersThatCanHear);

        if (rippleEffect)
        {
            rippleEffect.Emit(1);
        }

        foreach (var collider in colliders.Take(maxTargets).Except(ignoredColliders).OrderBy(x => (x.transform.position - transform.position).sqrMagnitude))
        {
            var hit = Physics2D.Raycast(transform.position, collider.transform.position - transform.position, noiseRadius, soundBlockingLayers);

            if(hit.collider){
                if((collider.transform.position - transform.position).magnitude > noiseRadius / 2){
                    continue;
                }
            }

            var hsm = collider.GetComponent<HSM>();
            if(hsm != null){
                if (noiseHeardEvent == NoiseEmitterEvents.noiseHeard)
                {
                    hsm.TriggerEvent(noiseHeardEvent.ToString(), 
                        new EventData{{"subject", interactableObject.transform.position}}
                    );
                }
                else
                {
                    hsm.TriggerEvent(noiseHeardEvent.ToString(), 
                        new EventData{{"subject", interactableObject}}
                    );
                }
            }
        }
    }
}
