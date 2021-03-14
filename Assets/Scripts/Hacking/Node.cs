using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Actions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Scripts.Hacking
{
    public class Node : MonoBehaviour
    {
        public GateType gateType;
        public float boardcastDelay;
        public bool moving;
        public int maxInputs = Int16.MaxValue;
        public int maxOutputs = Int16.MaxValue;
        public Image nodeImage;
        public Canvas nodeCanvas;
        public Color hoverColor;
        public Color selectedColor;
        public Color onColor;
        public Color offColor;
        public Color errorColor;
        public Color disabledColor;
        public SoundPreset deconnectedSound;

        internal AbstractGate gate;
        internal string nodeId;
        internal bool isVisible = true;
        internal bool connectedToInUI;
        internal DeviceUI deviceUI;
        internal bool accessible = true;

        private Dictionary<AbstractGate, Connection> connectionsTo = new Dictionary<AbstractGate, Connection>();
        private Transform mousePos;
        private NodeState currentState;
        private bool isHovered;
        internal bool rightClickDown;

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
            deviceUI = device?.GetComponent<DeviceUI>();
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
        }

        public void OnClick(BaseEventData eventData)
        {
            var pointerEvent = (PointerEventData)eventData;

            if (moving)
            {
                StartCoroutine(DisableMove());
                return;
            }

            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (!moving)
                {
                    moving = true;
                }
            }
            else
            {
                if (pointerEvent.button == PointerEventData.InputButton.Left)
                {
                    if (Network.Instance.selectedNode)
                    {
                        Network.Instance.ConnectionEnd(Network.Instance.selectedNode, this);
                        EventSystem.current.SetSelectedGameObject(null);
                    }
                    else if (maxOutputs > 0)
                    {
                        Network.Instance.selectedNode = this;
                        Network.Instance.ConnectionStart(this, true);
                    }
                }
                else
                {
                    rightClickDown = true;
                }
            }
        }

        public void OnUnClick(BaseEventData eventData)
        {
            var pointerEvent = (PointerEventData)eventData;
            if (pointerEvent.button == PointerEventData.InputButton.Right && rightClickDown)
            {
                deconnectedSound.Play(transform.position);
                DisconnectAll(true);
                rightClickDown = false;
            }
        }

        public void OnHoverEnter()
        {
            isHovered = true;
        }

        public void OnHoverExit()
        {
            isHovered = false;
            rightClickDown = false;
        }

        // Update is called once per frame
        void Update()
        {

            nodeCanvas.enabled = deviceUI?.selected ?? true;

            if (deviceUI && !deviceUI.selected)
            {
                return;
            }

            SetState(gate.currentValue ? NodeState.On : NodeState.Off);

            if (Network.Instance.selectedNode == this)
            {
                SetState(NodeState.Selected);
            }
            else
            {
                if (isHovered)
                {
                    SetState(NodeState.Hover);
                }

                if (Network.Instance.selectedNode && !Network.Instance.selectedNode.gate.CanConnect(gate))
                {
                    SetState(isHovered ? NodeState.Error : NodeState.Disabled);
                }
            }

            if (moving)
            {
                this.transform.position = mousePos.position;
            }
        }

        public void SetState(NodeState state)
        {
            if (currentState != state)
            {
                currentState = state;
                switch (state)
                {
                    case NodeState.Hover:
                        nodeImage.color = hoverColor;
                        break;

                    case NodeState.Selected:
                        nodeImage.color = selectedColor;
                        break;

                    case NodeState.On:
                        nodeImage.color = onColor;
                        break;

                    case NodeState.Off:
                        nodeImage.color = offColor;
                        break;

                    case NodeState.Error:
                        nodeImage.color = errorColor;
                        break;

                    case NodeState.Disabled:
                        nodeImage.color = disabledColor;
                        break;
                }
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

        internal void Remove()
        {
            Destroy(this.gameObject);
        }

        void OnDestroy()
        {
            DisconnectAll();
        }

        public void DisconnectAll(bool nodesOnly = false)
        {
            if (gate != null)
            {
                foreach (var parent in new List<AbstractGate>(gate.parents))
                {
                    if ((nodesOnly && parent.node) || !nodesOnly)
                    {
                        parent.Disconnect(this.gate);
                    }
                }

                foreach (var child in new List<AbstractGate>(gate.children))
                {
                    if ((nodesOnly && child.node) || !nodesOnly)
                    {
                        gate.Disconnect(child);
                    }
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
    }
}

