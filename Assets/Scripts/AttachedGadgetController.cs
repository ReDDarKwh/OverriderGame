using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Actions;
using UnityEngine;

public class AttachedGadgetController : MonoBehaviour
{

    public SpriteRenderer selection;
    public Device device;
    public Collider2D deviceCollider;

    private bool isAttached;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        selection.enabled = false;
    }

    internal void UpdateDeviceActions()
    {

    }

    internal void AttachGadget(GameObject prefab)
    {
        if (!isAttached)
        {
            var inst = Instantiate(prefab, transform.position, Quaternion.identity, transform);
            var childDevice = inst.GetComponentInChildren<Device>();
            childDevice.parentDevice = device;
            childDevice.attachedGadgetController = this;
            childDevice.isAttachedGadget = true;
            isAttached = true;

            if (deviceCollider != null)
            {
                deviceCollider.enabled = false;
            }
        }
    }

    internal void DetachGadget()
    {
        isAttached = false;

        if (deviceCollider != null)
        {
            deviceCollider.enabled = true;
        }
    }
}
