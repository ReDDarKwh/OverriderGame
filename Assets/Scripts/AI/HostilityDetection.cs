using System.Collections;
using System.Collections.Generic;
using Scripts.Actions;
using UnityEngine;

public class HostilityDetection : MonoBehaviour
{
    public Action shootingAction;
    public TargetingAction targetingAction;

    public string hostileLayerName;
    public string friendlyLayerName;

    private

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        SetHacked(
            (targetingAction.target == null && shootingAction.outputGate.currentValue) ||
            (
                shootingAction.outputGate.currentValue &&
                targetingAction.target.gameObject.layer == LayerMask.NameToLayer("Guard")
            )
        );
    }

    internal void SetHacked(bool hacked)
    {
        gameObject.layer = LayerMask.NameToLayer(hacked ? hostileLayerName : friendlyLayerName);
    }
}
