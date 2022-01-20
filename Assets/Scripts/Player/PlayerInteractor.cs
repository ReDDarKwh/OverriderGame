using System.Collections;
using System.Collections.Generic;
using Scripts.Actions;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    public SpriteRenderer selectionSprite;
    public Interactable interactable;
    public Device device;
    public LayerMask blockingLayers;
    public float cooldownTime;
    private bool playerInRange;
    private float lastTime;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInRange)
        {
            
            if (!device || device.playerHasRequiredSecurityAccess)
            {
                interactable.Use(null);
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
            var hit = false;//Physics2D.Raycast(collider.transform.position, transform.position - collider.transform.position, 1, blockingLayers).collider != null;
            playerInRange = !hit;
            selectionSprite.enabled = !hit;
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
