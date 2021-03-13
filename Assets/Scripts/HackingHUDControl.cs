using System.Collections;
using System.Collections.Generic;
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
        inst.moving = true;
        inst.Init(null, System.Guid.NewGuid().ToString());
    }
    public void CreateDevice(GameObject devicePrefab)
    {
        var inst = Instantiate(devicePrefab, mousePos.position + deviceSpawnOffset, Quaternion.identity, network.transform).GetComponent<DeviceUI>();
    }

    public void RemoveDevice()
    {
        if (network.lastDeviceMoved && network.lastDeviceMoved.UIDestroyable)
        {
            Destroy(network.lastDeviceMoved.gameObject);
        }
    }
}
