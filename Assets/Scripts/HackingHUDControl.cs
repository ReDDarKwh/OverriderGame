using System.Collections;
using System.Collections.Generic;
using Scripts.Hacking;
using UnityEngine;
using UnityEngine.UI;

public class HackingHUDControl : MonoBehaviour
{
    public Scripts.Hacking.Network network;
    public List<Image> ToolbarTiles;

    public void CreateNode(GameObject nodePrefab)
    {
        var inst = Instantiate(nodePrefab, network.transform).GetComponent<Node>();
        inst.moving = true;
        inst.Init(null, System.Guid.NewGuid().ToString());
    }
}
