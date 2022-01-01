using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scripts.UI 
{
    public class ContextMenuUIController : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
    {
        public GameObject ContextMenuItemPrefab;
        public GameObject ContextMenuSeparatorPrefab;
        private HackingHUDControl hackingHUDControl;

        // Start is called before the first frame update

        public struct ContextMenuItem{
            public string name;
            public UnityAction action;
        }

        public void SetItems(IEnumerable<ContextMenuItem> items, HackingHUDControl hackingHUDControl)
        {
            this.hackingHUDControl = hackingHUDControl;
            var itemCount = items.Count();
            foreach(var item in items.Select((value, i) => new { i, value })){
                var inst = Instantiate(ContextMenuItemPrefab, transform);
                var text = inst.GetComponentInChildren<TextMeshProUGUI>();
                var btn = inst.GetComponent<Button>();
                text.SetText(item.value.name);
                btn.onClick.AddListener(item.value.action);
                if(item.i < itemCount - 1){
                    Instantiate(ContextMenuSeparatorPrefab, transform);
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(hackingHUDControl != null){
                hackingHUDControl.SetDeviceInteractionActive(true);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(hackingHUDControl != null){
                hackingHUDControl.SetDeviceInteractionActive(false);
            }
        }
    }

}
