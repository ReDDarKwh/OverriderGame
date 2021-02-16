using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardRobotAnimControl : MonoBehaviour
{
    public Animator animator;
    public Transform legs;
    public Action chasingAction;
    public SpriteRenderer[] gunSprites;
    public Creature creature;
    public float damping;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Open", chasingAction.outputGate.currentValue);

        if (creature.nav.IsMoving())
        {
            var desiredRotQ = Quaternion.LookRotation(Vector3.forward, creature.headDir) * Quaternion.Euler(0, 0, 90);
            legs.rotation = Quaternion.Lerp(legs.rotation, desiredRotQ, Time.deltaTime * damping);
        }
    }

    public void ShowGuns()
    {
        foreach (var gun in gunSprites)
        {
            gun.gameObject.SetActive(true);
        }
    }

    public void HideGuns()
    {
        foreach (var gun in gunSprites)
        {
            gun.gameObject.SetActive(false);
        }
    }
}
