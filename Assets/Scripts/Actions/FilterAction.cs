using System.Collections;
using System.Collections.Generic;
using Scripts.Hacking;
using UnityEngine;

namespace Scripts.Actions
{
    public class FilterAction : Action
    {
        private DataGate filtersDataOutput;
        internal override void OnStart()
        {
            filtersDataOutput = new DataGate(false)
            {
                name = "filter",
                dataGateType = DataGate.DataGateType.Filter,
                saveData = true,
                dataType = DataGate.DataType.Filters
            };
            filtersDataOutput.SetValue(true);
            dataGates.Add(filtersDataOutput);
        }
    }
}