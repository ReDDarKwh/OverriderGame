using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesktopAnimControl : MonoBehaviour
{
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim.SetFloat("Speed", Random.Range(0.1f, 1.5f));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
