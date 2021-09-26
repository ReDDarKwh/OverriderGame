using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{

    public class InteractableUsedEvent: UnityEvent<Creature>{
    }

    public Transform interactionPos;
    public float InteractionTime;
    public InteractableUsedEvent onUsedWithCreature = new InteractableUsedEvent();
    public UnityEvent onUsed;
    public UnityEvent onError;

    internal void Use(Creature creature)
    {
        onUsedWithCreature.Invoke(creature);
        onUsed.Invoke();
    }
    internal void Error()
    {
        onError.Invoke();
    }

}