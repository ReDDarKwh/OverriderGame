using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Investigator : MonoBehaviour
{

    internal EnemySharedInfoManager sharedInfoManager;

    private HashSet<GameObject> investigatedObjects = new HashSet<GameObject>();

    void Start()
    {
        sharedInfoManager = GameObject.FindGameObjectWithTag("SceneManager")
        .GetComponent<EnemySharedInfoManager>();
    }

    public bool CanBeInvestigated(GameObject gameObject)
    {
        var investigated = sharedInfoManager.IsObjectAlreadyInvestigated(gameObject);
        var wasInvestigated = investigatedObjects.Contains(gameObject);

        if (!investigated)
        {
            sharedInfoManager.AddInvestigatedObject(gameObject);
            investigatedObjects.Add(gameObject);
        }

        return !investigated || wasInvestigated;
    }

    void OnDestroy()
    {
        foreach (var io in investigatedObjects)
        {
            sharedInfoManager.RemoveInvestigatedObject(io);
        }
    }

}
