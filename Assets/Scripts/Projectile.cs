using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    public Rigidbody2D rb;
    public float speed;
    public Collider2D ignoredCollider;
    public float inactiveTime;
    public GameObject hitEffect;
    public LayerMask collisionLayers;
    private float lifeStart;
    private bool damageDone;

    void Start()
    {
        lifeStart = Time.time;
    }

    void FixedUpdate()
    {
        var dir = transform.rotation * Vector3.right;
        rb.MovePosition(rb.transform.position + dir * speed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider != ignoredCollider || ignoredCollider == null)
        {
            if (!damageDone)
            {
                collider.GetComponent<Killable>()?.InflictDamage(damage);
                damageDone = true;
                if (hitEffect)
                {
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 1, collisionLayers);
                    if (hit)
                    {
                        Instantiate(hitEffect, hit.point, transform.rotation);
                    }
                }
            }
            Destroy(this.gameObject);
        }
    }
}
