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
    public ParticleSystem rippleEffect;
    public GameObject interactableObject;

    public void EmitNoise()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, noiseRadius, layersThatCanHear);

        if (rippleEffect)
        {
            rippleEffect.Emit(1);
        }

        foreach (var collider in colliders.Take(maxTargets))
        {
            if (noiseHeardEvent == "NoiseHeard")
            {
                CustomEvent.Trigger(collider.gameObject, noiseHeardEvent, interactableObject.transform.position);
            }
            else
            {
                CustomEvent.Trigger(collider.gameObject, noiseHeardEvent, interactableObject);
            }
        }
    }
}
