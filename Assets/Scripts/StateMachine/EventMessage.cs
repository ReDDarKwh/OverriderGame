using UnityEngine;

namespace Scripts.StateMachine
{
    public class EventMessage
    {
        public static EventMessage EmptyMessage = new EventMessage();
        public object data;
    }
}