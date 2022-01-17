﻿using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Hacking;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Vectrosity;
using TMPro;
using Network = Scripts.Hacking.Network;
using UnityEngine.Events;
using UnityEditor;

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
        public bool runPreconnectionsOnStart;
        public int accessLevel;
        public SoundPreset deconnectedSound;

        internal bool playerCanAccess;
        public Device parentDevice;
        internal AttachedGadgetController attachedGadgetController;
        internal bool isAttachedGadget;
        internal Dictionary<Action, Dictionary<string, Node>> nodesPerAction;
        internal bool initiated;

        private Dictionary<string, GameObject> currentActionDisplays = new Dictionary<string, GameObject>();
        private Transform mousePos;

        internal UnityEvent OnPlayerCanAccess = new UnityEvent();

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
                UpdateAccessLevel(0);
                mousePos = GameObject.FindGameObjectWithTag("MousePos").transform;
                initiated = true;
                Network.Instance.OnUpdateAccessLevel.AddListener(UpdateAccessLevel);

                if(runPreconnectionsOnStart && Debug.isDebugBuild){
                    StartCoroutine(DelayedPreconnection());
                }
            }
        }

        private IEnumerator DelayedPreconnection(){
            yield return new WaitForSeconds(5f);
            ExecutePreConnection();
        }

        internal void UpdateAccessLevel(int accessLevelId)
        {
            if(accessLevelId == -1){
                SetNodesPlayerAccessible(false);
                playerCanAccess = false;
            } else if(accessLevelId == accessLevel)
            {
                SetNodesPlayerAccessible(true);
                playerCanAccess = true;
            }
            
            OnPlayerCanAccess.Invoke();
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
                var fromNode = (preconnection.fromDevice ?? this).nodesPerAction[preconnection.fromAction][preconnection.fromNodeName];
                var toNode = (preconnection.toDevice ?? this).nodesPerAction[preconnection.toAction][preconnection.toNodeName];
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


        void OnDrawGizmos(){

            Handles.Label(transform.position, accessLevel.ToString(), new GUIStyle{alignment = TextAnchor.LowerCenter});
        }
    }
}