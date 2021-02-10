using UnityEngine.Events;

[System.Serializable]
public struct ActionEventItem
{
    public Action action;
    public UnityEvent onOutputTrue;
    public UnityEvent onOutputFalse;
}