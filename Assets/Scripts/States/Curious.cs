using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class Curious : MonoBehaviour
{
    private GameObject target;
    public void StateEnter()
    {
        target = Variables.Object(gameObject).Get<GameObject>("target");
        Variables.Object(gameObject).Set("lastTargetPos", target.transform.position);
    }
}
