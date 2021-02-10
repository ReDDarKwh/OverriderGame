using System;
using System.Collections;
using System.Collections.Generic;
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

    internal Node AddDataGate(DataGate dataGate, string actionid)
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

        return inst.node;
    }
}
