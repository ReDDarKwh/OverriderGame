using System;
using System.Collections;
using System.Collections.Generic;
using Lowscope.Saving;
using Scripts.Hacking;
using UnityEngine;
using UnityEngine.UI;

public class HackingHUDControl : MonoBehaviour
{
    public Scripts.Hacking.Network network;
    public List<Image> ToolbarTiles;
    public Transform mousePos;
    public Vector3 deviceSpawnOffset;

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
