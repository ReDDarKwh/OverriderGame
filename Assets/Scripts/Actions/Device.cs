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
        public bool disableLine;
        public Transform actionWindow;
        public Canvas actionDisplayCanvas;
        public RectTransform actionDisplayContainer;
        public Transform minimizeConnectionTransform;
        public Preconnection[] preconnections;
        public TextMeshProUGUI title;
        public GameObject deviceCircle;
        public int accessLevel;
        public GameObject AccessDeniedDisplay;
        public TextMeshProUGUI AccessDeniedText;

        internal bool playerCanAccess = true;
        private Dictionary<string, GameObject> currentActionDisplays = new Dictionary<string, GameObject>();
        internal Device parentDevice;
        internal bool uiVisible;
        internal AttachedGadgetController attachedGadgetController;
        internal bool isAttachedGadget;
        private bool isMoving;
        private Transform mousePos;
        private Vector3 pos;
        private Vector3 diff;
        private LineFactory lineFactory;
        private Line line;

        internal bool isAnchored;
        private Dictionary<Action, Dictionary<string, Node>> nodesPerAction;

        // Start is called before the first frame update
        void Start()
        {
            UpdateActionDisplays();
            UpdateAccessLevel();

            pos = actionWindow.transform.position;

            if (parentDevice != null)
            {
                actionWindow.gameObject.SetActive(false);
            }
            else
            {
                if (!disableLine)
                {
                    lineFactory = GameObject.FindGameObjectWithTag("LineFactory").GetComponent<LineFactory>();
                    line = lineFactory.GetLine(Vector2.zero, Vector2.zero, 0.02f, Color.green);
                }
                else
                {
                    deviceCircle.SetActive(false);
                }
            }

            mousePos = GameObject.FindGameObjectWithTag("MousePos").transform;
        }

        // Update is called once per frame
        void Update()
        {
            if (line != null)
            {
                line.start = transform.position;
                line.end = uiVisible ? actionDisplayContainer.position +
                new Vector3(actionDisplayContainer.rect.size.x / 2, -actionDisplayContainer.rect.size.y / 2) : actionDisplayCanvas.transform.position;
            }
        }

        void LateUpdate()
        {
            if (isMoving)
            {
                pos = mousePos.transform.position + diff;
                actionWindow.position = pos;
            }
            else
            {
                if (isAnchored)
                {
                    actionWindow.position = pos;
                }
            }
        }

        internal void UpdateAccessLevel()
        {
            if (Network.Instance.accessLevel < this.accessLevel && playerCanAccess)
            {
                SetNodesPlayerAccessible(false);
                playerCanAccess = false;
                AccessDeniedDisplay.SetActive(true);
                title.SetText(deviceName + " (Secured)");
                AccessDeniedText.SetText($"Level {this.accessLevel}");
            }
            else
            {
                SetNodesPlayerAccessible(true);
                playerCanAccess = true;
                AccessDeniedDisplay.SetActive(false);
                title.SetText(deviceName);
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

        public void ToggleIsAnchored()
        {
            isAnchored = !isAnchored;

            if (isAnchored)
            {
                pos = actionWindow.transform.position;
            }
        }

        public void ToggleMinimize()
        {
            uiVisible = !uiVisible;
            UpdateVisibility();
        }

        public void SetIsMoving(bool isMoving)
        {
            this.isMoving = isMoving;
            if (isMoving)
            {
                diff = actionWindow.position - mousePos.transform.position;
                actionDisplayCanvas.sortingOrder++;
            }
        }

        private void UpdateVisibility()
        {
            if (uiVisible)
            {
                ShowHUD();
            }
            else
            {
                HideHUD();
            }
        }

        private HackUI[] GetUIHack()
        {
            return GetComponentsInChildren<HackUI>();
        }


        private void ShowHUD()
        {
            foreach (var h in GetUIHack())
            {
                h.Show();
            }
        }

        private void HideHUD()
        {
            foreach (var h in GetUIHack())
            {
                h.Hide();
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
                        var actionid = deviceId.uniqueId + action.actionName;

                        currentActionDisplays.Add(action.actionName, inst.gameObject);
                        actionDisplay.name = action.actionName;
                        actionDisplay.device = parentDevice ?? this;

                        actionDisplay.outputNode.Init(this, actionid + "output");
                        actionDisplay.inputNode.Init(this, actionid + "input");

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
                            dataNodes.Add(dataGate.name, actionDisplay.AddDataGate(dataGate, actionid));
                        }
                    }
                }

                UpdateVisibility();
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
                Network.Instance.Connect(fromNode, toNode);
            }
        }

        void OnDestroy()
        {
            UpdateActionDisplays();

            if (line != null)
            {
                line.gameObject.SetActive(false);
            }

            if (isAttachedGadget)
            {
                attachedGadgetController.DetachGadget();
            }
        }

        internal void Disable()
        {
        }
    }

}
