using System.Collections;
using System.Collections.Generic;
using Scripts.Hacking;
using UnityEngine;
using System.Linq;

namespace Scripts.Actions
{
    public class ExternalLogicAction : Action
    {
        public List<string> dataInputsNames;

        [System.NonSerialized]
        public Dictionary<string, IList<GameObject>> dataInputs = new Dictionary<string, IList<GameObject>>();
        public Dictionary<string, bool> dataInputsHasData = new Dictionary<string, bool>();

        internal override void OnStart()
        {
            foreach (var dIn in dataInputsNames)
            {
                var gate = new DataGate { name = dIn, dataGateType = DataGate.DataGateType.Input };
                dataGates.Add(gate);
                dataInputs.Add(gate.name, null);
                dataInputsHasData.Add(gate.name, false);
                gate.ValueHasChanged += targetsDataInput_ValueChanged;
            };
        }

        private void targetsDataInput_ValueChanged(object sender, System.EventArgs e)
        {
            var dataGate = (DataGate)sender;
            dataInputs[dataGate.name] = dataGate.GetData<GameObject>().ToList();
            dataInputsHasData[dataGate.name] = dataInputs[dataGate.name].FirstOrDefault() != null;
        }

    }
}