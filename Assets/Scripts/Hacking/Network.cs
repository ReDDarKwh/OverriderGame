﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Actions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.Hacking
{
    public class Network : MonoBehaviour
    {
        public Connection connection;
        public GameObject connectionPrefab;
        public Node mousePosNode;
        public static Network Instance = null;
        public SelectionSquareUIController selectionController;
        public LayerMask nodeLayerMask;


        internal int accessLevel;
        internal DeviceUI lastDeviceMoved;
        internal int baseDeviceSortingOrder;
        internal IEnumerable<Node> selectedNodes;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void UpdateAccessLevels(int accessLevel)
        {
            foreach (var device in GetComponentsInChildren<Device>())
            {
                this.accessLevel = accessLevel;
                device.UpdateAccessLevel();
            }
        }

        public void DeselectSelectedNodes(bool destroyConnection = true, bool makeSound = false)
        {
            selectedNodes = null;
            if (destroyConnection)
            {
                if (makeSound)
                {
                    connection.PlayDeconnectedSound();
                }
                Destroy(connection.gameObject);
            }
            connection = null;
        }

        internal void Connect(Node from, Node to, bool soundOn)
        {
            ConnectionStart(from, soundOn);
            ConnectionEnd(from, to);
        }

        internal void ConnectionStart(Node from, bool soundOn)
        {
            connection = Instantiate(connectionPrefab, transform).GetComponent<Connection>();
            connection.soundOn = soundOn;
            connection.start = from;
            connection.end = mousePosNode;
        }

        internal void ConnectionEnd(Node from, Node to)
        {
            var connected = from.Connect(to, connection);
            connection.Connected();
            DeselectSelectedNodes(!connected);
        }

        private void RemoveSelectedNodes()
        {
            foreach(var selectedNode in selectedNodes){
                selectedNode.Remove();
            }
            DeselectSelectedNodes(true, true);
        }

        // Update is called once per frame
        void Update()
        {

            if (selectedNodes != null)
            {
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButton(1))
                {
                    DeselectSelectedNodes(true, true);
                }
              
                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    RemoveSelectedNodes();
                }
            }

            if (Debug.isDebugBuild)
            {
                if (Input.GetKeyDown(KeyCode.U))
                {
                    UpdateAccessLevels(100);
                }

                if (Input.GetKeyDown(KeyCode.I))
                {
                    foreach(var device in GetComponentsInChildren<Device>()){
                        device.ExecutePreConnection();
                    }
                }
            }
        }

        // EVENTS

        internal void OnNodeClickDown(BaseEventData eventData, Node node)
        {
            Debug.Log("OnNodeClickDown");
        }
        internal void OnNodeClickUp(BaseEventData eventData, Node node)
        {
            Debug.Log("OnNodeClickUp");
        }
        internal void OnNodeHoverEnter(Node node)
        {
            Debug.Log("OnNodeHoverEnter");
        }
        internal void OnNodeHoverExit(Node node)
        {
            Debug.Log("OnNodeHoverExit");
        }
        internal void OnNodeBeginDrag(BaseEventData eventData, Node node)
        {
            Debug.Log("OnNodeBeginDrag");
        }
        internal void OnNodeDrag(BaseEventData eventData, Node node)
        {
            Debug.Log("OnNodeDrag");
        }
        internal void OnNodeEndDrag(BaseEventData eventData, Node node)
        {
            Debug.Log("OnNodeEndDrag");
        }


        public void OnBackgroundClickDown(BaseEventData eventData)
        {
            Debug.Log("OnBackgroundClickDown");
        }
        public void OnBackgroundClickUp(BaseEventData eventData)
        {
            Debug.Log("OnBackgroundClickUp");
        }
        public void OnBackgroundBeginDrag(BaseEventData eventData)
        {
            Debug.Log("OnBackgroundBeginDrag");
        }
        public void OnBackgroundDrag(BaseEventData eventData)
        {
            Debug.Log("OnBackgroundDrag");
        }
        public void OnBackgroundEndDrag(BaseEventData eventData)
        {
            Debug.Log("OnBackgroundEndDrag");
        }


    }
}
