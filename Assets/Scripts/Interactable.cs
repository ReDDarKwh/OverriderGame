using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public float InteractionTime;
    public UnityEvent onUsed;
    public UnityEvent onError;
    internal void Use()
    {
        onUsed.Invoke();
    }
    internal void Error()
    {
        onError.Invoke();
    }
}