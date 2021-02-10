using UnityEngine;
using System.Linq;

namespace Scripts.Hacking
{
    public class NotGate : AbstractGate
    {
        public NotGate()
        {
            maxInputs = 1;
        }

        public override void UpdateValue()
        {
            var parentValue = (parents.FirstOrDefault()?.currentValue ?? false);

            if (currentValue != !parentValue)
            {
                currentValue = !parentValue;
                BroadcastValue(!parentValue);
            }
        }
    }
}

