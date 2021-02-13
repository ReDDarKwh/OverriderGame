using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent onUsed;
    internal void Use()
    {
        onUsed.Invoke();
    }
}