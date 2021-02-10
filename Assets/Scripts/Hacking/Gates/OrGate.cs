using System.Linq;
using UnityEngine;

namespace Scripts.Hacking
{
    public class OrGate : AbstractGate
    {
        public OrGate(int maxInputs, int maxOutputs) : base(maxInputs, maxOutputs)
        {
        }

        public OrGate()
        {
        }

        public override void UpdateValue()
        {
            var condition = parents.Any(x => x.currentValue);

            if (currentValue != condition)
            {
                currentValue = condition;
                BroadcastValue(condition);
            }
        }
    }
}

