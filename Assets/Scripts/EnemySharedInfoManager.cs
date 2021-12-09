using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySharedInfoManager : MonoBehaviour
{
    private HashSet<GameObject> investigatedObjects = new HashSet<GameObject>();
    public Dictionary<string, GameObject> objectRepo = new Dictionary<string, GameObject>();
    private Enemy[] enemies;

    void Start(){
        enemies = FindObjectsOfType<Enemy>();
    }

    public IEnumerable<Enemy> GetAllAliveEnemies(){
        return enemies.Where(x => !(x?.killable.dead ?? false));
    }

    public bool IsObjectAlreadyInvestigated(GameObject o)
    {
        return investigatedObjects.Contains(o);
    }

    public void AddInvestigatedObject(GameObject o)
    {
        investigatedObjects.Add(o);
    }

    public void RemoveInvestigatedObject(GameObject o)
    {
        investigatedObjects.Remove(o);
    }

}
