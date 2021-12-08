using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIElementDragger : EventTrigger
{
    public BaseEventDataEvent OnDown = new BaseEventDataEvent();
    public BaseEventDataEvent OnExit = new BaseEventDataEvent();

    public override void OnPointerDown(PointerEventData eventData)
    {
        OnDown.Invoke(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        OnExit.Invoke(eventData);
    }
}
