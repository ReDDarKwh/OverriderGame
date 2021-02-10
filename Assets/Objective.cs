using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct Objective
{
    public string name;
    public ObjectiveType objectiveType;
    public UnityEvent onComplete;

    // type Kill only
    public Killable target;

    // type goToPosition only
    public TriggerZone triggerZone;
}
