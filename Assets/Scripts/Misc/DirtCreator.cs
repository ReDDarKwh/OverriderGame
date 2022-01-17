using System.Collections;
using System.Collections.Generic;
using Lowscope.Saving;
using UnityEngine;

public class DirtCreator : MonoBehaviour
{
    public GameObject dirtPrefab;
    public string dirtPrefabPath;
    public int quantityMax;
    public int quantityMin;
    public bool manual;
    public float time;
    public float range;
    private float creationTime;

    // Start is called before the first frame update
    void Start()
    {
        creationTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!manual && Time.time - creationTime > time)
        {
            Run();
        }
    }

    public void Run()
    {
        for (var i = 0; i < Random.Range(quantityMin, quantityMax); i++)
        {
            var angle = Random.value * Mathf.PI * 2;
            var dis = Random.value * range;

            SaveMaster.SpawnSavedPrefab(
                dirtPrefabPath, 
                transform.position + new Vector3(Mathf.Cos(angle) * dis, Mathf.Sin(angle) * dis), 
                Quaternion.identity
            );
        }

        this.enabled = false;
    }
}
