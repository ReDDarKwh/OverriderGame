using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceSelection : MonoBehaviour
{
    internal bool selected;
    private bool canBeSelected;

    void Update()
    {
        if (canBeSelected)
        {
            if (Input.GetMouseButtonDown(0))
            {
                selected = !selected;
            }
        }
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "MousePos")
        {
            canBeSelected = true;
        }
    }
    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "MousePos")
        {
            canBeSelected = false;
        }
    }
}
