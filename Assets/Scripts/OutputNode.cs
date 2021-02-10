using System.Collections;
using System.Collections.Generic;
using Scripts.Hacking;
using UnityEngine;
using Network = Scripts.Hacking.Network;

public class OutputNode : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite outputSprite;
    public Sprite connectedSprite;
    public Node node;

    // Update is called once per frame
    void Update()
    {
        if (node.network.selectedNode == node || node.gate.children.Count > 0)
        {
            spriteRenderer.sprite = connectedSprite;
        }
        else
        {
            spriteRenderer.sprite = outputSprite;
        }
    }
}
