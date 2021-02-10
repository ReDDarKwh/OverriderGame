using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySharedInfoManager : MonoBehaviour
{
    private HashSet<GameObject> investigatedObjects = new HashSet<GameObject>();

    public bool IsObjectAlreadyInvestigated(GameObject o)
    {
        return investigatedObjects.Contains(o);
    }

    public void AddInvestigedObject(GameObject o)
    {
        investigatedObjects.Add(o);
    }

    public void RemoveInvestigedObject(GameObject o)
    {
        investigatedObjects.Remove(o);
    }

}
