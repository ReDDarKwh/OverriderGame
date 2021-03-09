using System.Collections;
using System.Collections.Generic;
using Scripts.Actions;
using UnityEngine;

public class DeadState : MonoBehaviour
{
    public GameObject explodeEffect;
    public GameObject deadBodyPrefab;

    // Start is called before the first frame update
    public void StateEnter(DamageType damageType)
    {
        if (damageType == DamageType.Explosion)
        {
            Instantiate(explodeEffect, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
        else
        {
            Instantiate(deadBodyPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
