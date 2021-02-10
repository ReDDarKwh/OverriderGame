using System.Collections;
using System.Collections.Generic;
using Scripts.Actions;
using UnityEngine;

namespace Scripts.Hacking
{
    public abstract class HackUI : MonoBehaviour
    {
        public Device device;
        public abstract void Show();
        public abstract void Hide();

    }
}
