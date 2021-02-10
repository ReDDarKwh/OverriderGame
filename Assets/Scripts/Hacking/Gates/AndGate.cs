
using UnityEngine;
using System.Linq;

namespace Scripts.Hacking
{
    public class AndGate : AbstractGate
    {
        public override void UpdateValue()
        {
            var condition = parents.All(x => x.currentValue);

            if (currentValue != condition)
            {
                currentValue = condition;
                BroadcastValue(condition);
            }
        }
    }
}
