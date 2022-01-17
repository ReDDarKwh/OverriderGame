using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class GameSaveUIController : MonoBehaviour, ISelectHandler, IPointerClickHandler
{
    public Image image;
    public Text text;
    internal SaveLoadUIController saveLoadUIController;
    internal int slotNumber;
    public Selectable selectable;

    public void OnPointerClick(PointerEventData eventData)
    {
        selectable.Select();
    }

    public void OnSelect(BaseEventData eventData)
    {
        saveLoadUIController.SelectSaveSlot(this);
    }
}
