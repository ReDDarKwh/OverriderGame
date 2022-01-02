using System.Collections;
using System.Collections.Generic;
using Scripts.Hacking;
using UnityEngine;

namespace Scripts.Actions
{
    public class GenericAction : Action
    {
        private DataGate genericDataOutput;
        internal override void OnStart()
        {
            genericDataOutput = new DataGate (false) { name = "Device", dataGateType = DataGate.DataGateType.Output };
            dataGates.Add(genericDataOutput);
            genericDataOutput.SetData(new List<GameObject> { gameObject });
        }
    }
}