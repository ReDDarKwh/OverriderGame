using System;
using System.Collections;
using System.Collections.Generic;
using Lowscope.Saving;
using Lowscope.Saving.Components;
using Scripts.Actions;
using Scripts.Hacking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionNodeDisplay : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Node inputNode;
    public Node outputNode;

    public GameObject dataIoContainer;

    public GameObject dataInputPrefab;
    public GameObject dataOutputPrefab;
    public GameObject dataFilterPrefab;

    public CanvasHackUI hackUI;
    public Device device;

    // Start is called before the first frame update
    void Start()
    {
        if (name == "")
        {
            text.gameObject.SetActive(false);
        }
        else
        {
            text.text = name;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    internal Node AddDataInterface(DataGate dataGate, string actionid)
    {
        GameObject prefab = null;
        switch (dataGate.dataGateType)
        {
            case DataGate.DataGateType.Input:
                prefab = dataInputPrefab;
                break;
            case DataGate.DataGateType.Output:
                prefab = dataOutputPrefab;
                break;
            case DataGate.DataGateType.Filter:
                prefab = dataFilterPrefab;
                break;
            default:
                prefab = dataInputPrefab;
                break;

        }
        var inst = Instantiate(prefab, dataIoContainer.transform).GetComponent<DataIoDisplay>();

        if (inst.dataName)
        {
            inst.dataName.text = dataGate.name;
        }
        inst.node.gate = dataGate;
        inst.node.Init(device, actionid + dataGate.name);

        inst.gameObject.SetActive(!dataGate.isHiddenFromPlayer);
        
        if(dataGate.dataGateType == DataGate.DataGateType.Filter){
            var s = inst.GetComponent<FilterOutputDisplay>();
            string mono = (s as MonoBehaviour).name;
            var ds = device.GetComponent<Saveable>();
            ds.AddSaveableComponent(string.Format("Dyn-{0}-{1}", mono, device.deviceId), s, true);
        }

        return inst.node;
    }
}
