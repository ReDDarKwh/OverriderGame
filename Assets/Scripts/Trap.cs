using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{

    public TrapState state;
    public float damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<Killable>()?.InflictDamage(damage);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public enum TrapState
    {
        Armed,
        UnArmed
    }
}
