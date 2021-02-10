using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtCreator : MonoBehaviour
{
    public GameObject dirtPrefab;
    public ParticleSystem particle;
    public int quantityMax;
    public int quantityMin;
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
        if (Time.time - creationTime > time)
        {
            Run();
        }
    }

    private void Run()
    {
        for (var i = 0; i < Random.Range(quantityMin, quantityMax); i++)
        {
            var angle = Random.value * Mathf.PI * 2;
            var dis = Random.value * range;

            Instantiate(dirtPrefab, transform.position + new Vector3(Mathf.Cos(angle) * dis, Mathf.Sin(angle) * dis), Quaternion.identity);
        }

        this.enabled = false;
    }

}
