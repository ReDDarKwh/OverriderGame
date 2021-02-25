﻿using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;
using System.Linq;

public class ChasingState : MonoBehaviour
{
    public Creature creature;
    public ExternalLogicAction chasingAction;
    public float chasingSpeed;
    public SoundPreset chasingSound;

    public void MakeChasingSound()
    {
        if (chasingSound)
        {
            SoundManager.Instance.Make(chasingSound, transform.position);
        }
    }

    public void StateEnter()
    {
        if (chasingAction != null)
        {
            chasingAction.actionGate.SetValue(true);
        }
    }

    public void StateUpdate()
    {
    }

    public void StateExit()
    {
    }
}