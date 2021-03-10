using System.Collections;
using System.Collections.Generic;
using Scripts.Actions;
using UnityEngine;

public class PlayerInteractable : MonoBehaviour
{
    public SpriteRenderer selectionSprite;
    public Interactable interactable;
    public Device device;
    public float cooldownTime;
    private bool playerInRange;
    private float lastTime;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInRange)
        {
            if (!device || device.playerCanAccess)
            {
                interactable.Use();
            }
            else
            {
                interactable.Error();
            }
        }

    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            playerInRange = true;
            selectionSprite.enabled = true;
        };
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            playerInRange = false;
            selectionSprite.enabled = false;
        };
    }

}
