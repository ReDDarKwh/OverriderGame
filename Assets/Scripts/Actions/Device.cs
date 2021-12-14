using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Hacking;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Vectrosity;
using TMPro;
using Network = Scripts.Hacking.Network;

namespace Scripts.Actions
{
    public class Device : MonoBehaviour
    {
        public string deviceName;
        public GameObject ActionNodePrefab;
        public UniqueId deviceId;
        public bool disableOutput;
        public bool disableInput;
        public RectTransform actionDisplayContainer;
        public Preconnection[] preconnections;
        public int accessLevel;
        public SoundPreset deconnectedSound;

        internal bool playerCanAccess = true;
        internal Device parentDevice;
        internal AttachedGadgetController attachedGadgetController;
        internal bool isAttachedGadget;
        internal Dictionary<Action, Dictionary<string, Node>> nodesPerAction;
        internal bool initiated;

        private Dictionary<string, GameObject> currentActionDisplays = new Dictionary<string, GameObject>();
        private Transform mousePos;

        // Start is called before the first frame update
        void Start()
        {
            Init();
        }

        public void Init()
        {
            if (!initiated)
            {
                UpdateActionDisplays();
                UpdateAccessLevel();
                mousePos = GameObject.FindGameObjectWithTag("MousePos").transform;
                initiated = true;
            }
        }

        internal void UpdateAccessLevel()
        {
            if (Network.Instance.accessLevel < this.accessLevel && playerCanAccess)
            {
                SetNodesPlayerAccessible(false);
                playerCanAccess = false;
            }
            else
            {
                SetNodesPlayerAccessible(true);
                playerCanAccess = true;
            }
        }

        private void SetNodesPlayerAccessible(bool accessible)
        {
            foreach (var actions in nodesPerAction)
            {
                foreach (var node in actions.Value.Values)
                {
                    node.SetPlayerAccessible(accessible);
                }
            }
        }

        public void DisconnectAll(bool nodesOnly)
        {
            if (playerCanAccess)
            {
                deconnectedSound.Play(transform.position);
                foreach (var action in nodesPerAction)
                {
                    foreach (var node in action.Value.Values)
                    {
                        node.DisconnectAll(nodesOnly);
                    }
                }
            }
        }

        public void DisconnectAllForce()
        {
            foreach (var action in nodesPerAction)
            {
                foreach (var node in action.Value.Values)
                {
                    node.DisconnectAll();
                }
            }
        }

        public void UpdateActionDisplays()
        {
            var nodesPerAction = new Dictionary<Action, Dictionary<string, Node>>();

            if (parentDevice == null)
            {
                var actions = GetComponentsInChildren<Action>();
                var angle = Mathf.PI * 2 / actions.Length;

                foreach (var action in new List<string>(currentActionDisplays.Keys).Except(actions.Select(x => x.actionName)))
                {
                    Destroy(currentActionDisplays[action]);
                    currentActionDisplays.Remove(action);
                }

                for (var i = 0; i < actions.Length; i++)
                {
                    var action = actions[i];
                    var dataNodes = new Dictionary<string, Node>();
                    nodesPerAction.Add(action, dataNodes);

                    if (!currentActionDisplays.ContainsKey(action.actionName))
                    {
                        var inst = Instantiate(ActionNodePrefab, actionDisplayContainer);
                        var actionDisplay = inst.GetComponent<ActionNodeDisplay>();
                        var actionId = deviceId.uniqueId + action.actionName;

                        currentActionDisplays.Add(action.actionName, inst.gameObject);
                        actionDisplay.name = action.actionName;
                        actionDisplay.device = parentDevice ?? this;

                        actionDisplay.outputNode.Init(this, actionId + "output");
                        actionDisplay.inputNode.Init(this, actionId + "input");

                        action.inputGate = actionDisplay.inputNode.gate;
                        action.outputGate = actionDisplay.outputNode.gate;

                        dataNodes.Add("Input", actionDisplay.inputNode);
                        dataNodes.Add("Output", actionDisplay.outputNode);

                        action.Init();

                        if (disableInput || action.disableInput)
                        {
                            actionDisplay.inputNode.gameObject.SetActive(false);
                        }
                        if (disableOutput || action.disableOutput)
                        {
                            actionDisplay.outputNode.gameObject.SetActive(false);
                        }

                        foreach (var dataGate in action.dataGates)
                        {
                            dataNodes.Add(dataGate.name, actionDisplay.AddDataInterface(dataGate, actionId));
                        }
                    }
                }

                actionDisplayContainer.anchoredPosition = Vector2.zero;
            }
            else
            {
                parentDevice.UpdateActionDisplays();
            }

            this.nodesPerAction = nodesPerAction;
        }

        public void ExecutePreConnection()
        {
            foreach (var preconnection in preconnections)
            {
                var fromNode = nodesPerAction[preconnection.fromAction][preconnection.fromNodeName];
                var toNode = nodesPerAction[preconnection.toAction][preconnection.toNodeName];
                Network.Instance.Connect(fromNode, toNode, false);
            }
        }

        void OnDestroy()
        {
            UpdateActionDisplays();

            if (isAttachedGadget)
            {
                attachedGadgetController.DetachGadget();
            }
        }

        void OnDisable()
        {
            DisconnectAllForce();
        }
    }
}
