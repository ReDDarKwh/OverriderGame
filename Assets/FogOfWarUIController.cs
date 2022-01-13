using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarUIController : MonoBehaviour
{
    public ForOfWar[] zones;
    public void Show(){

        foreach(var fow in zones){
            fow.animator.SetTrigger("show");
            fow.image.raycastTarget = false;
            fow.col.enabled = false;
        }
    }
}
