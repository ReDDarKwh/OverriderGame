using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class CuriousState : MonoBehaviour
{
    private GameObject target;
    public void StateEnter()
    {
        target = Variables.Object(gameObject).Get<GameObject>("target");

        if (target)
        {
            Variables.Object(gameObject).Set("lastTargetPos", target.transform.position);
        }
    }
}
