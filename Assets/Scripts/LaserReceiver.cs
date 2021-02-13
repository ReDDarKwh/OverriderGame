using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserReceiver : MonoBehaviour
{
    public Transform laserEnd;
    public ExternalLogicAction activeAction;
    public float detectDistance;

    // Update is called once per frame
    void Update()
    {
        activeAction.actionGate.SetValue(laserEnd.gameObject.activeInHierarchy && (laserEnd.position - transform.position).magnitude < detectDistance);
    }
}
