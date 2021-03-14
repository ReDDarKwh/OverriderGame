using System.Collections;
using System.Collections.Generic;
using Scripts.Hacking;
using UnityEngine;

public class FilterAction : Action
{
    private DataGate filtersDataInput;
    internal override void OnStart()
    {
        filtersDataInput = new DataGate
        {
            name = "filter",
            dataGateType = DataGate.DataGateType.Filter,
            saveData = true,
            dataType = DataGate.DataType.Filters
        };
        filtersDataInput.SetValue(true);
        dataGates.Add(filtersDataInput);
    }
}
