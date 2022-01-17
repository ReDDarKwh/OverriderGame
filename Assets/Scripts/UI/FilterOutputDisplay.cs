using System;
using System.Collections;
using System.Collections.Generic;
using Lowscope.Saving;
using Scripts.Hacking;
using UnityEngine;
using UnityEngine.UI;

public class FilterOutputDisplay : MonoBehaviour, ISaveable
{
    public DataIoDisplay ioDisplay;
    public LayerMask mask;
    public Toggle guards;
    public Toggle player;

     [Serializable]
    public struct SaveData
    {
        public LayerMask mask;
    }

    public void ToggleLayer(string mask)
    {
        if (this.mask == (this.mask | (1 << LayerMask.NameToLayer(mask))))
        {
            this.mask = this.mask & ~(1 << LayerMask.NameToLayer(mask));
        }
        else
        {
            this.mask = this.mask | (1 << LayerMask.NameToLayer(mask));
        }

        UpdateMask();
    }

    void UpdateMask()
    {
        if (ioDisplay.node.gate != null)
        {
            ((DataGate)ioDisplay.node.gate).SetData((int)this.mask);
        }
    }

    void UpdateUI()
    {
        guards.SetIsOnWithoutNotify(this.mask == (this.mask | (1 << LayerMask.NameToLayer("HackedGuard"))));
        guards.SetIsOnWithoutNotify(this.mask == (this.mask | (1 << LayerMask.NameToLayer("Guard"))));
        player.SetIsOnWithoutNotify(this.mask == (this.mask | (1 << LayerMask.NameToLayer("Player"))));
    }

    // Start is called before the first frame update
    void Start()
    {
        ((DataGate)ioDisplay.node.gate).ValueHasChanged += dataGate_ValueChanged;
        UpdateMask();
    }

    private void dataGate_ValueChanged(object sender, EventArgs e)
    {
        var newVal = ((DataGate)ioDisplay.node.gate).GetCurrentSingleData<int>();
        if (newVal != mask)
        {
            this.mask = newVal;
            UpdateUI();
        }
    }

    // Update is called once per frame
    void Update()
    {
        ioDisplay.node.gate.SetValue(this.mask != 0);
    }

    public string OnSave()
    {
        return JsonUtility.ToJson(new SaveData {mask = mask});
    }

    public void OnLoad(string data)
    {
        mask = JsonUtility.FromJson<SaveData>(data).mask;
        UpdateUI();
    }

    public bool OnSaveCondition(bool v)
    {
        return true;
    }
}
