using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForOfWar : MonoBehaviour
{
    public Image image;
    public Animator animator;
    public Collider2D col;

    public void OnEndFade(){
        this.gameObject.SetActive(false);
    }
}
