using System.Collections.Generic;
using UnityEngine;


namespace Scripts.Gadgets
{
    public class Gadget : MonoBehaviour
    {
        public string gadgetName;
        public GameObject handObject;
        internal GadgetModule[] modules;
        internal GadgetRepo repo;

        void Start()
        {
            modules = GetComponents<GadgetModule>();
            foreach (var m in modules)
            {
                m.repo = repo;
            }
        }
    }
}
