using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Hacking;
using UnityEngine;
using System.Linq;

public abstract class Action : MonoBehaviour
{
    public string actionName;
    public bool disableOutput;
    public bool disableInput;
    internal AbstractGate inputGate;
    public AbstractGate outputGate;
    internal OrGate actionGate;
    private OrGate cGate;
    internal abstract void OnStart();
    private bool isInitiated;
    internal IList<DataGate> dataGates = new List<DataGate>();
    internal bool hacked;

    public event EventHandler AfterInit;

    public void Init()
    {
        if (isInitiated)
        {
            return;
        }

        actionGate = new OrGate();
        inputGate.ConnectedTo += gate_ConnectedTo;
        inputGate.DisconnectedFrom += gate_DisconnectedFrom;

        var notC = new NotGate();
        var notCAndA = new AndGate();
        var cAndB = new AndGate();
        var notCAndA_or_cAndB = new OrGate();

        cGate = new OrGate();

        cGate.Connect(notC);
        actionGate.Connect(notCAndA);
        notC.Connect(notCAndA);
        notCAndA.Connect(notCAndA_or_cAndB);

        inputGate.Connect(cAndB, true);
        cGate.Connect(cAndB);
        cAndB.Connect(notCAndA_or_cAndB);

        notCAndA_or_cAndB.Connect(outputGate, true);

        OnStart();

        isInitiated = true;

        OnAfterInit(EventArgs.Empty);
    }
    private void gate_DisconnectedFrom(object sender, EventArgs e)
    {
        if (inputGate.parents.Count == 0)
        {
            cGate.SetValue(false);
            hacked = false;
        }
    }
    private void gate_ConnectedTo(object sender, EventArgs e)
    {
        if (inputGate.parents.Count == 1)
        {
            cGate.SetValue(true);
            hacked = true;
        }
    }
    protected virtual void OnAfterInit(EventArgs e)
    {
        EventHandler handler = AfterInit;
        handler?.Invoke(this, e);
    }
}

