﻿using System.Collections;
using System.Collections.Generic;
using Scripts.Hacking;
using UnityEngine;

public class GenericAction : Action
{
    private DataGate genericDataOutput;
    internal override void OnStart()
    {
        genericDataOutput = new DataGate { name = "Device", dataGateType = DataGate.DataGateType.Output };
        dataGates.Add(genericDataOutput);
        genericDataOutput.SetData(new List<GameObject> { gameObject });
    }
}
