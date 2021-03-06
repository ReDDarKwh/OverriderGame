using System.Collections;
using System.Collections.Generic;
using Scripts.Actions;
using UnityEngine;

namespace Scripts.Hacking
{
    public abstract class HackUI : MonoBehaviour
    {
        public Device device;
        public int hideLevel;
        public bool hidden;
        internal bool partialHide;
        public virtual void Show()
        {

        }
        public virtual void Hide()
        {

        }



    }
}
