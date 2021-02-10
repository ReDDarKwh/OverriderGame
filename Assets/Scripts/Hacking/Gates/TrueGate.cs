using UnityEngine;
using System.Linq;

namespace Scripts.Hacking
{
    public class TrueGate : AbstractGate
    {
        public TrueGate()
        {
            maxInputs = 0;
            currentValue = true;
        }

        public override void UpdateValue()
        {
            BroadcastValue(currentValue);
        }
    }
}

