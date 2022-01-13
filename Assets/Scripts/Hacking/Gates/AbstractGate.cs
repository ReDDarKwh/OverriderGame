using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Hacking
{
    public abstract class AbstractGate
    {
        public Node node;
        internal HashSet<AbstractGate> children = new HashSet<AbstractGate>();
        internal HashSet<AbstractGate> parents = new HashSet<AbstractGate>();
        public bool currentValue;
        public bool isHiddenFromPlayer;

        internal int maxInputs = Int16.MaxValue;
        internal int maxOutputs = Int16.MaxValue;

        public abstract void UpdateValue();
        public event EventHandler ConnectedTo;
        public event EventHandler DisconnectedFrom;

        public event EventHandler<ConnectionEventArgs> ConnectionCompleted;
        public event EventHandler<ConnectionEventArgs> DeconnectionCompleted;
        public event EventHandler ValueHasChanged;

        public AbstractGate()
        {
        }

        public AbstractGate(int maxInputs, int maxOutputs)
        {
            this.maxInputs = maxInputs;
            this.maxOutputs = maxOutputs;
        }

        protected virtual void OnConnectedTo(EventArgs e)
        {
            EventHandler handler = ConnectedTo;
            handler?.Invoke(this, e);
        }

        protected virtual void OnDisconnectedFrom(EventArgs e)
        {
            EventHandler handler = DisconnectedFrom;
            handler?.Invoke(this, e);
        }

        protected virtual void OnConnectionCompleted(ConnectionEventArgs e)
        {
            EventHandler<ConnectionEventArgs> handler = ConnectionCompleted;
            handler?.Invoke(this, e);
        }

        protected virtual void OnDeconnectionCompleted(ConnectionEventArgs e)
        {
            EventHandler<ConnectionEventArgs> handler = DeconnectionCompleted;
            handler?.Invoke(this, e);
        }

        protected virtual void OnValueChanged(EventArgs e)
        {
            EventHandler handler = ValueHasChanged;
            handler?.Invoke(this, e);
        }

        public virtual bool CanConnect(AbstractGate gate)
        {
            return children.Count + 1 <= maxOutputs && gate.CanBeConnectedTo(this);
        }

        public virtual bool CanBeConnectedTo(AbstractGate gate)
        {
            return parents.Count + 1 <= maxInputs;
        }

        protected void ChildConnection(AbstractGate parent)
        {
            parents.Add(parent);
            UpdateValue();
            OnConnectedTo(EventArgs.Empty);
        }

        protected void ChildDisconnection(AbstractGate parent)
        {
            parents.Remove(parent);
            UpdateValue();
            OnDisconnectedFrom(EventArgs.Empty);
        }

        public bool Connect(AbstractGate gate, bool forceConnect = false)
        {
            var added = false;
            if (forceConnect || CanConnect(gate))
            {
                added = children.Add(gate);
                if (added)
                {
                    gate.ChildConnection(this);
                    OnConnectionCompleted(new ConnectionEventArgs { Gate = gate });
                }
            }
            return added;
        }

        public bool Disconnect(AbstractGate gate)
        {
            var removed = children.Remove(gate);
            if (removed)
            {
                gate.ChildDisconnection(this);
                OnDeconnectionCompleted(new ConnectionEventArgs { Gate = gate });
            }
            return removed;
        }

        public void BroadcastValue(bool value)
        {
            // The node will call broadcast instead. 
            if (node == null)
            {
                Broadcast(value);
            }
            OnValueChanged(EventArgs.Empty);
        }

        internal void Broadcast(bool value)
        {
            foreach (var child in children)
            {
                child.UpdateValue();
            }
        }

        internal void SetValue(bool value)
        {
            if (value != currentValue)
            {
                currentValue = value;
                BroadcastValue(value);
            }
        }
    }
}

