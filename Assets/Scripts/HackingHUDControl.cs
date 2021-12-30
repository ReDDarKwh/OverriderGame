﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lowscope.Saving;
using Scripts.Hacking;
using UnityEngine;
using UnityEngine.UI;
using static ContextMenuUIController;

public class HackingHUDControl : MonoBehaviour
{
    public Scripts.Hacking.Network network;
    public List<Image> ToolbarTiles;
    public Transform mousePos;
    public Vector3 deviceSpawnOffset;
    public GameObject contextMenuContainerPrefab;
    private ContextMenuUIController currentContextMenu;
    public LayerMask deviceLayerMask;
    private bool isDeviceInteractionActive = true;

    public void CreateContextMenu(IEnumerable<ContextMenuItem> items){

        var ctxMenu = Instantiate(contextMenuContainerPrefab, Input.mousePosition, Quaternion.identity, transform).GetComponent<ContextMenuUIController>();
        ctxMenu.SetItems(items, this);
        currentContextMenu = ctxMenu;
    }

    void LateUpdate(){
           
        if(Input.GetMouseButtonUp(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)){
            if(currentContextMenu != null){
                Destroy(currentContextMenu.gameObject);
                currentContextMenu = null;
                return;
            }
        }

        if(isDeviceInteractionActive){
            var devicesUnderMouse = Physics2D.OverlapPointAll(mousePos.position, deviceLayerMask).Select(x => x.GetComponent<DeviceUI>()).ToList();

            foreach(var d in devicesUnderMouse){
                d.OnHover();                    
            }

            if(Input.GetMouseButtonUp(0)){
                if(devicesUnderMouse.Count() == 1){
                    foreach(var d in devicesUnderMouse){
                        d.ToggleSelected();                      
                    }
                } else if(devicesUnderMouse.Any()){
                    CreateContextMenu(devicesUnderMouse.Select(x => {
                        return new ContextMenuItem{name = x.device.deviceName, action = ()=> {
                                x.ToggleSelected();
                                SetDeviceInteractionActive(true);
                            }};
                    }));
                }
            }
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

}
