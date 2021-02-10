using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Gadgets
{
    public class GadgetController : MonoBehaviour
    {
        public GadgetRepo repo;
        public Transform hand;
        private Gadget currentGadget;
        private GameObject handObject;

        // Start is called before the first frame update
        void Start()
        {
            repo.gadgetSelected += repo_GadgetSelected;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void repo_GadgetSelected(object sender, EventArgs e)
        {
            // Clean up old gadget
            if (currentGadget != null)
            {
                Destroy(handObject);
            }

            if (repo.currentGadget != null)
            {
                // Setup new gadget
                currentGadget = repo.currentGadget;
                // place in hand
                handObject = Instantiate(currentGadget.handObject, hand);
            }
        }
    }
}
