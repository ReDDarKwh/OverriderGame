using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Explosion : MonoBehaviour
{
    public MilkShake.ShakePreset shakePreset;
    public SoundPreset explosionSound;
    public float damage;
    public float radius;
    public LayerMask affected;
    // Start is called before the first frame update
    void Start()
    {
        MilkShake.Shaker.ShakeAll(shakePreset);
        SoundManager.Instance.Make(explosionSound, transform.position);
    }

    public void DealDamage()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, radius, affected);
        foreach (var killable in colliders.Select(x => x.GetComponent<Killable>()).Where(x => x != null))
        {
            var dis = (killable.transform.position - transform.position).magnitude;
            killable.InflictDamage(damage * (1 - Mathf.InverseLerp(0, radius, dis)), DamageType.Explosion);
        }
    }

    public void AnimationEnd()
    {
        Destroy(this.gameObject);
    }
}
