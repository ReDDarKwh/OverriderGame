using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using System.Linq;


public class NoiseEmitter : MonoBehaviour
{
    public LayerMask layersThatCanHear;
    public NoiseEmitterEvents noiseHeardEvent = NoiseEmitterEvents.noiseHeard;
    public float noiseRadius = 10;
    public int maxTargets = 100;
    public ParticleSystem rippleEffect;
    public GameObject interactableObject;
    public Collider2D[] ignoredColliders;

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

        foreach (var collider in colliders.Take(maxTargets).Except(ignoredColliders))
        {

            var hsm = collider.GetComponent<HSM>();

            if (noiseHeardEvent == NoiseEmitterEvents.noiseHeard)
            {
                hsm.TriggerEvent(noiseHeardEvent.ToString(), 
                    new Dictionary<string, object>{{"subject", interactableObject.transform.position}}
                );
            }
            else
            {
                hsm.TriggerEvent(noiseHeardEvent.ToString(), 
                    new Dictionary<string, object>{{"subject", interactableObject}}
                );
            }
        }
    }
}
