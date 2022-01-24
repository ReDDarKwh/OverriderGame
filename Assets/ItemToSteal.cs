using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemToSteal : MonoBehaviour
{
    public float cost;

    private ObjectiveManager om;

    void Start(){
        om = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<ObjectiveManager>();
    }

    public void Steal(){
        om.AddMoney(cost);
        gameObject.SetActive(false);
    }

}
