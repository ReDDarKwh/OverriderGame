using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Gadgets.Modules;
using UnityEngine;

namespace Scripts.Gadgets
{
    public class GadgetRepo : MonoBehaviour
    {
        public List<Gadget> gadgetsPrefabs;
        public Transform gadgetParent;
        internal List<Gadget> instanciatedGadgets = new List<Gadget>();
        internal Gadget currentGadget;
        internal event EventHandler gadgetSelected;
        private KeyCode[] gadgetKeys = new KeyCode[]{
                KeyCode.Alpha1,
                KeyCode.Alpha2,
                KeyCode.Alpha3,
                KeyCode.Alpha4,
                KeyCode.Alpha5,
                KeyCode.Alpha6,
                KeyCode.Alpha7,
                KeyCode.Alpha8,
                KeyCode.Alpha9,
                KeyCode.Alpha0,
            };


        void OnGadgetSelected(EventArgs e)
        {
            EventHandler handler = gadgetSelected;
            handler?.Invoke(this, e);
        }

        void Start()
        {
            foreach (var gadget in gadgetsPrefabs)
            {
                var g = Instantiate(gadget, gadgetParent);
                g.repo = this;
                g.gameObject.SetActive(false);
                instanciatedGadgets.Add(g.GetComponent<Gadget>());
            }

        }

        void Update()
        {
            for (var i = 0; i < gadgetKeys.Length; i++)
            {
                if (Input.GetKeyDown(gadgetKeys[i]))
                {
                    if(i > instanciatedGadgets.Count - 1){
                        SelectGadget(null);
                    } else {
                        SelectGadget(instanciatedGadgets[i] == currentGadget ? null : instanciatedGadgets[i]);
                    }
                    break;
                }
            }
        }

        public void SelectGadget(Gadget g)
        {
            if (currentGadget != null)
            {
                currentGadget.gameObject.SetActive(false);
            }

            currentGadget = g;

            if (currentGadget != null)
            {
                currentGadget.gameObject.SetActive(true);
            }
            OnGadgetSelected(EventArgs.Empty);
        }
    }
}
