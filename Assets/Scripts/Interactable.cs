using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
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