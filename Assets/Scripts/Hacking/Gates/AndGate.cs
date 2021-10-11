
using UnityEngine;
using System.Linq;

namespace Scripts.Hacking
{
    public class AndGate : AbstractGate
    {
        public override void UpdateValue()
        {
            var condition = parents.Any() && parents.All(x => x.currentValue);

            if (currentValue != condition)
            {
                currentValue = condition;
                BroadcastValue(condition);
            }
        }
    }
}
