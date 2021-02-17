using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Actions;
using UnityEngine;

namespace Scripts.Hacking
{
    public class Node : MonoBehaviour
    {
        public GateType gateType;
        public GameObject connectionPrefab;
        public SpriteRenderer spriteRenderer;
        public float interactionRadius;


        public float boardcastDelay;

        private NodeState currentState;
        private Dictionary<AbstractGate, Connection> connectionsTo = new Dictionary<AbstractGate, Connection>();
        private Transform mousePos;
        internal AbstractGate gate;
        internal string nodeId;
        public bool moving;
        public int maxInputs = Int16.MaxValue;
        public int maxOutputs = Int16.MaxValue;

        internal bool isVisible = true;
        internal bool connectedToInUI;

        public HackUI hackUI;
        internal Device device;
        internal bool accessible = true;

        // Start is called before the first frame update

        void SetGate()
        {
            switch (gateType)
            {
                case GateType.OR:
                    gate = new OrGate(maxInputs, maxOutputs);
                    break;
                case GateType.AND:
                    gate = new AndGate();
                    break;
                case GateType.NOT:
                    gate = new NotGate();
                    break;
                case GateType.TRUE:
                    gate = new TrueGate();
                    break;
            }
        }

        public void Init(Device device, string uniqueId = null)
        {
            if (gate == null) SetGate();

            if (uniqueId != null)
            {
                nodeId = uniqueId;
            }

            gate.node = this;
            gate.DeconnectionCompleted += gate_DeconnectionCompleted;
            gate.ValueHasChanged += gate_ValueChange;
            mousePos = GameObject.FindGameObjectWithTag("MousePos").transform;
            this.device = device;
        }

        private void UpdateConnectionColor(Connection c)
        {
            c.lineRenderer.startColor = gate.currentValue ? Color.green : Color.grey;
        }

        private void gate_ValueChange(object sender, EventArgs e)
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(BroadcastValues());
            }
            else
            {
                BroadcastValues();
            }
        }

        private IEnumerator BroadcastValues()
        {
            yield return new WaitForSeconds(boardcastDelay);
            gate.Broadcast(gate.currentValue);
            foreach (var c in connectionsTo.Values)
            {
                UpdateConnectionColor(c);
            }
        }

        // Update is called once per frame
        void Update()
        {

            if (!isVisible || !accessible)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0) && moving)
            {
                StartCoroutine(DisableMove());
            }


            SetState(gate.currentValue ? NodeState.On : NodeState.Off);

            if (Network.Instance.selectedNode != this)
            {
                if ((mousePos.position - transform.position).magnitude < interactionRadius)
                {
                    SetState(NodeState.Hover);
                    if (Network.Instance.selectedNode != null && !Network.Instance.selectedNode.gate.CanConnect(gate))
                    {
                        SetState(NodeState.Error);
                    }

                    if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftControl) && !moving)
                    {
                        moving = true;
                    }
                }
            }
            else
            {
                SetState(NodeState.Selected);
            }


            if (moving)
            {
                this.transform.position = mousePos.position;
            }
        }

        internal void SetPlayerAccessible(bool accessible)
        {
            this.accessible = accessible;
        }

        public IEnumerator DisableMove()
        {
            yield return new WaitForEndOfFrame();
            moving = false;
        }

        public void SetState(NodeState state)
        {
            if (currentState != state)
            {
                currentState = state;
                switch (state)
                {
                    case NodeState.Hover:
                        spriteRenderer.color = Color.gray;
                        break;

                    case NodeState.Selected:
                        spriteRenderer.color = Color.yellow;
                        break;

                    case NodeState.On:
                        spriteRenderer.color = Color.green;
                        break;

                    case NodeState.Off:
                        spriteRenderer.color = Color.white;
                        break;

                    case NodeState.Error:
                        spriteRenderer.color = Color.red;
                        break;
                }
            }
        }

        internal void Remove()
        {
            Destroy(this.gameObject);
        }

        void OnDestroy()
        {
            if (gate != null)
            {
                foreach (var parent in new List<AbstractGate>(gate.parents))
                {
                    parent.Disconnect(this.gate);
                }

                foreach (var child in new List<AbstractGate>(gate.children))
                {
                    gate.Disconnect(child);
                }
            }
        }

        internal void Disconnect(Node node)
        {
            gate.Disconnect(node.gate);
        }

        internal bool Connect(Node node, Connection connection)
        {
            var result = gate.Connect(node.gate);
            if (result)
            {
                connection.end = node;
                connectionsTo.Add(node.gate, connection);
                connectedToInUI = true;
            }
            return result;
        }

        private void gate_DeconnectionCompleted(object sender, ConnectionEventArgs e)
        {
            if (connectionsTo.TryGetValue(e.Gate, out var connection))
            {
                connectionsTo.Remove(e.Gate);
                Destroy(connection.gameObject);
                connectedToInUI = connectionsTo.Count > 0;
            }
        }

        internal void Hide()
        {
            spriteRenderer.enabled = false;
            isVisible = false;
        }

        internal void Show()
        {
            spriteRenderer.enabled = device == null ? true : device.uiVisible;
            isVisible = true;
        }
    }
}

