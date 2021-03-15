using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bolt;
using Lowscope.Saving;
using UnityEngine;

public class PatrollingState : MonoBehaviour, ISaveable
{
    public Creature creature;
    public Transform[] waypoints;
    public Transform stationnaryTransform;
    public GameObject waypointPrefab;
    internal int currentPoint = -1;

    void Init()
    {
        if (stationnaryTransform)
        {
            var inst = Instantiate(waypointPrefab, stationnaryTransform.position, stationnaryTransform.rotation)
                .GetComponent<Transform>();

            waypoints = new Transform[] { inst };
        }
    }

    internal Vector3 GetNextPoint()
    {
        return waypoints[currentPoint = (currentPoint + 1) % waypoints.Count()].position;
    }

    public void AtStationnaryOutpost()
    {
        creature.headDir = waypoints[0].right;
    }

    public Vector3? StateEnter()
    {
        if (stationnaryTransform)
        {
            if (waypoints.Count() == 0)
            {
                Init();
            }
            return GetNextPoint();
        }
        else
        {
            if (waypoints.Count() > 0)
            {
                return GetNextPoint();
            }
        }

        return null;
    }

    public void StateUpdate()
    {
    }

    public void StateExit()
    {
    }

    public string OnSave()
    {
        return JsonUtility.ToJson(currentPoint);
    }

    public void OnLoad(string data)
    {
        currentPoint = JsonUtility.FromJson<int>(data);
    }

    public bool OnSaveCondition()
    {
        return true;
    }
}
