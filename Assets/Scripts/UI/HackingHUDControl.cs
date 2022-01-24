using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lowscope.Saving;
using Scripts.Hacking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Scripts.UI.ContextMenuUIController;
using Network = Scripts.Hacking.Network;

namespace Scripts.UI
{
    public class HackingHUDControl : MonoBehaviour
    {
        public Scripts.Hacking.Network network;
        public List<Image> ToolbarTiles;
        public Transform mousePos;
        public Vector3 deviceSpawnOffset;
        public GameObject contextMenuContainerPrefab;
        private ContextMenuUIController currentContextMenu;
        public LayerMask deviceLayerMask;
        public GameObject hackedLevelsContainer;
        public GameObject hackedLevelPrefab;
        public float doubleClickInterval;
        public TextMeshProUGUI moneyText;

        private bool isDeviceInteractionActive = true;
        private float lastMouseLeftUpTime = -1;
        private bool successfulDefaultIOConnection;
        private bool selectDeviceOnMouseUp;

        public void ChangeMoneyAmount(float amount){
            moneyText.text =  "$" + amount.ToString();    
        }

        void Start()
        {
            Network.Instance.OnUpdateAccessLevel.AddListener((int accessLevel) => { 
                if (accessLevel == -1) 
                { 
                    ClearHackedAccessLevels(); 
                } 
                else 
                { 
                    AddHackedAccessLevel(Network.Instance.accessLevels[accessLevel]); 
                }; 
            });
        }

        public void AddHackedAccessLevel(Color color)
        {
            var inst = Instantiate(hackedLevelPrefab, hackedLevelsContainer.transform);
            inst.GetComponentInChildren<AccessLevelUIController>().img.color = color;
        }

        public void ClearHackedAccessLevels()
        {
            foreach (Transform child in hackedLevelsContainer.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        public void CreateContextMenu(IEnumerable<ContextMenuItem> items)
        {

            var ctxMenu = Instantiate(contextMenuContainerPrefab, Input.mousePosition, Quaternion.identity, transform).GetComponent<ContextMenuUIController>();
            ctxMenu.SetItems(items, this);
            currentContextMenu = ctxMenu;
        }

        void LateUpdate()
        {

            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            {
                if (currentContextMenu != null)
                {
                    Destroy(currentContextMenu.gameObject);
                    currentContextMenu = null;
                    return;
                }
            }

            if (isDeviceInteractionActive)
            {
                var devicesUnderMouse = Physics2D.OverlapPointAll(mousePos.position, deviceLayerMask).Select(x => x.GetComponent<DeviceUI>()).Where(x => x.isPlayerAccessable).ToList();

                foreach (var d in devicesUnderMouse)
                {
                    d.OnHover();
                }

                if(Input.GetMouseButtonUp(1)){
                    foreach(var d in devicesUnderMouse){
                        d.device.DisconnectAll(true);
                    }
                }

                if (Input.GetMouseButtonDown(0) && devicesUnderMouse.Any())
                {
                    if (lastMouseLeftUpTime != -1 && Time.unscaledTime - lastMouseLeftUpTime < doubleClickInterval && devicesUnderMouse.Where(x=> !x.selected).Any())
                    {
                        if(!network.isConnecting){
                            if(successfulDefaultIOConnection)
                                network.CancelLastConnections();
                        } else {
                            if(successfulDefaultIOConnection)
                                network.RemoveConnections();
                        }                        
                        
                        selectDeviceOnMouseUp = true;
                        
                        lastMouseLeftUpTime = -1;
                    }
                    else
                    {
                        var d = devicesUnderMouse?.FirstOrDefault() ?? null;
                        successfulDefaultIOConnection = false;

                        if(d){
                            var success = network.RequestDeviceDefaultIOConnection(d.device);
                            if(!success){
                                selectDeviceOnMouseUp = true;   
                            }
                            successfulDefaultIOConnection = success;
                        }

                        lastMouseLeftUpTime = Time.unscaledTime;
                    }
                }

                if(selectDeviceOnMouseUp && Input.GetMouseButtonUp(0)){

                    selectDeviceOnMouseUp = false;
                    SelectDevice(devicesUnderMouse);
                }

            }
        }

        private void SelectDevice(List<DeviceUI> devicesUnderMouse)
        {
            if (devicesUnderMouse.Count() == 1)
            {
                foreach (var d in devicesUnderMouse)
                {
                    d.Open();
                }
            }
            else if (devicesUnderMouse.Any())
            {
                var items = devicesUnderMouse.Select(x =>
                {
                    return new ContextMenuItem
                    {
                        name = x.device.deviceName,
                        action = () =>
                        {
                            x.Open();
                            SetDeviceInteractionActive(true);
                        }
                    };
                }).OrderBy(x => x.name).ToList();

                CreateContextMenu(
                    new List<ContextMenuItem>(){
                            new ContextMenuItem{
                                name = "Open all",
                                action = ()=>{
                                    foreach(var a in items){
                                        a.action();
                                    }
                                }
                            }
                    }.Concat(items)
                );
            }
        }

        internal void SetDeviceInteractionActive(bool isActive)
        {
            isDeviceInteractionActive = isActive;
            mousePos.GetComponent<CircleCollider2D>().enabled = isActive;
        }

        public void CreateNode(GameObject nodePrefab)
        {
            var inst = Instantiate(nodePrefab, network.transform).GetComponent<Node>();
            inst.Init(null, System.Guid.NewGuid().ToString());
            inst.transform.position = mousePos.position;

            network.SelectNode(inst, true);
            network.StartNodeDrag();
        }
        public void CreateDevice(GameObject devicePrefab)
        {
            var inst = Instantiate(devicePrefab, mousePos.position + deviceSpawnOffset, Quaternion.identity, network.transform).GetComponent<DeviceUI>();
        }

        public void CreateSavedDevice(string devicePrefabPath)
        {
            var inst = SaveMaster.SpawnSavedPrefab(devicePrefabPath, mousePos.position + deviceSpawnOffset, Quaternion.identity, network.transform).GetComponent<UniqueId>();
            inst.uniqueId = Guid.NewGuid().ToString();
        }

        public void RemoveDevice()
        {
            if (network.lastDeviceMoved && network.lastDeviceMoved.UIDestroyable)
            {
                Destroy(network.lastDeviceMoved.gameObject);
            }
        }

        public void HideAllDeviceUIs()
        {
            foreach (var dui in network.GetComponentsInChildren<DeviceUI>())
            {
                if (dui.selected)
                {
                    dui.ToggleSelected();
                }
            }
        }

    }

}
