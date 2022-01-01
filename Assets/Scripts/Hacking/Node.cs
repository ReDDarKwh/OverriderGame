using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lowscope.Saving;
using Scripts.Actions;
using Scripts.UI;
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
        private Vector3 movingOffset;
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
        public EventTrigger evtTrigger;

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


        [Serializable]
        public struct SaveData
        {
            public bool gateValue;
        }

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

        // Update is called once per frame
        void Update()
        {
            nodeCanvas.enabled = deviceUI?.selected ?? true;

            if (deviceUI && !deviceUI.selected || !accessible)
            {
                Network.Instance.DeselectNode(this);
                return;
            }

            if(currentState == NodeState.Selected){
                nodeImage.color = selectedColor;
            } else {
                nodeImage.color = gate.currentValue ? onColor : offColor;
                if (isHovered)
                {
                    nodeImage.color = hoverColor;
                }
                if (Network.Instance.isConnecting && (Network.Instance.selectedNodes?.Any() ?? false) && !Network.Instance.selectedNodes.All(x => x.gate.CanConnect(gate)))
                {
                    nodeImage.color =  isHovered ? errorColor : disabledColor;
                }
            }

            if (moving)
            {
                this.transform.position = mousePos.position + movingOffset;
            }
        }

        public void SetMoving(bool isMoving, Vector3 offset){

            if(deviceUI != null){
                deviceUI.SetIsMoving(isMoving);
            } else {
                moving = isMoving;
                movingOffset = offset;
            }
        }

        public void SetState(NodeState state)
        {
            if (currentState != state)
            {
                currentState = state;
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

        public string OnSave()
        {
            return JsonUtility.ToJson(new SaveData{gateValue = gate.currentValue});
        }

        public void OnLoad(string data)
        {
            gate.currentValue = JsonUtility.FromJson<SaveData>(data).gateValue;      
        }

        public bool OnSaveCondition()
        {
            return this != null && this.gameObject.activeSelf;
        }

        
        // EVENTS
        public void OnClick(BaseEventData eventData)
        {
            Network.Instance.OnNodeClickDown(eventData, this);    
        }

        public void OnUnClick(BaseEventData eventData)
        {
            Network.Instance.OnNodeClickUp(eventData, this);    
        }

        public void OnHoverEnter()
        {
            isHovered = true;
        }

        public void OnHoverExit()
        {
            isHovered = false;
        }

        public void OnBeginDrag(BaseEventData eventData)
        {
            Network.Instance.OnNodeBeginDrag(eventData, this);            
        }

        public void OnDrag(BaseEventData eventData)
        {
            Network.Instance.OnNodeDrag(eventData, this);   
        }

        public void OnEndDrag(BaseEventData eventData)
        {
            Network.Instance.OnNodeEndDrag(eventData, this);   
        }

    }
}

