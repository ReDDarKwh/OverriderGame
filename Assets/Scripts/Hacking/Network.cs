﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Actions;
using Scripts.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Scripts.Hacking
{
    public class Network : MonoBehaviour
    {
        public GameObject connectionPrefab;
        public Node mousePosNode;
        public static Network Instance = null;
        public SelectionSquareUIController selectionController;
        public LayerMask nodeLayerMask;
        public Color[] accessLevels;
        public SoundPreset disconnectSound;
        public Transform padlockMouseCursor;

        internal bool isConnecting;
        internal HashSet<int> hackedAccessLevels = new HashSet<int>();
        internal DeviceUI lastDeviceMoved;
        internal int baseDeviceSortingOrder;
        internal HashSet<Node> selectedNodes = new HashSet<Node>();
        internal bool isConnectionSelectionEnabled = true;
        internal bool isNodeDragStarted;
        
        internal AccessLevelEvent OnUpdateAccessLevel = new AccessLevelEvent();

        private bool isSelectionDragStarted;
        private Vector3 selectionStartPos;
        private Dictionary<Node, Connection> connectionBySelectedNode;
        private Node selectedNodeFromHUD;
        private int padlockCount;

        public class AccessLevelEvent : UnityEvent<int>{};

        IEnumerator Start(){
            yield return null;
            UpdateAccessLevels(0);
        }

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

        public void UpdateAccessLevels(int accessLevelId)
        {
            if(!hackedAccessLevels.Contains(accessLevelId)){
                OnUpdateAccessLevel.Invoke(accessLevelId);
                hackedAccessLevels.Add(accessLevelId);
            }
        }

        public void ClearAccessLevels()
        {
            hackedAccessLevels.Clear();           
            OnUpdateAccessLevel.Invoke(-1);
        }
       
        internal void Connect(Node from, Node to, bool soundOn)
        {
            ConnectionEnd(from, to, ConnectionStart(from, soundOn), soundOn);
        }

        public void ShowPadlockCursor(bool hasAccess){
            if(!hasAccess){
                padlockMouseCursor.gameObject.SetActive(true);
                Cursor.visible = false;
                padlockCount ++;
            }
        }

        public void HidePadlockCursor(bool hasAccess){
            padlockCount = Mathf.Max(0, padlockCount - 1) ;
            if(!hasAccess && padlockCount == 0){
                padlockMouseCursor.gameObject.SetActive(false);
                Cursor.visible = true;
            }
        }

        internal Connection ConnectionStart(Node from, bool soundOn, bool reversed = false)
        {
            if(from.gate.isHiddenFromPlayer)
            {
                return null;
            }

            var connection = Instantiate(connectionPrefab, transform).GetComponent<Connection>();
            connection.soundOn = soundOn;

            if(reversed){
                connection.start = mousePosNode;
                connection.end = from;
            } else {
                connection.start = from;
                connection.end = mousePosNode;
            }

            connection.OnHoverEnter.AddListener(ShowPadlockCursor);
            connection.OnHoverExit.AddListener(HidePadlockCursor);

            connection.reversed = reversed;

            return connection;
        }

        internal void ConnectionEnd(Node from, Node to, Connection connection, bool soundOn = true)
        {
            var connected = from.Connect(to, connection);

            if(connection){
                connection.Connected();
            }

            DeselectSelectedNodes();
            if(!connected && connection){
                RemoveConnection(connection, soundOn);
            }
        }

        internal void CancelLastConnections()
        {
            DeselectSelectedNodes();

            if(connectionBySelectedNode == null || !connectionBySelectedNode.Any()){
                return;
            }

            foreach(var keyval in connectionBySelectedNode){
                keyval.Value.start.Disconnect(keyval.Value.end);
                SelectNode(keyval.Key);
            }

            StartOrEndConnect(null);
        }

        private void RemoveConnection(Connection connection, bool soundOn = true)
        {
            if(soundOn)
                connection.PlayDisconnectionSound();
            Destroy(connection.gameObject);
        }

        internal bool RequestDeviceDefaultIOConnection(Device device)
        {
            var ioAction = isConnecting? device.defaultIO.inputAction : device.defaultIO.outputAction;
            if(!ioAction || !device.playerHasRequiredSecurityAccess){
                return false;
            }

            var node = isConnecting? ioAction.inputGate.node: ioAction.outputGate.node;
            
            if(!isConnecting){
                DeselectSelectedNodes();
                SelectNode(node);
            }

            StartOrEndConnect(node);
            return true;
        }

        private void RemoveSelectedNodes()
        {
            if(isConnecting){
                RemoveConnections();
            }

            foreach(var selectedNode in selectedNodes){
                if(!selectedNode.deviceUI)
                    selectedNode.Remove();
            }

            DeselectSelectedNodes();
        }

        // Update is called once per frame
        void Update()
        {

            padlockMouseCursor.position = Input.mousePosition;

            if (selectedNodes != null)
            {
                if (Input.GetButtonDown("HideHacking"))
                {
                    if(isConnecting){
                        RemoveConnections();
                    }   
                    DeselectSelectedNodes();
                }

                if(Input.GetMouseButtonUp(0) && selectedNodeFromHUD != null){

                    StartOrEndConnect(selectedNodeFromHUD);
                    selectedNodeFromHUD = null;
                }
              
                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    RemoveSelectedNodes();
                }
            }

            if (Debug.isDebugBuild)
            {
                if (Input.GetKeyDown(KeyCode.Y))
                {
                    for(var i = 0; i< accessLevels.Count(); i++){
                        UpdateAccessLevels(i);
                    }
                }
                
                if (Input.GetKeyDown(KeyCode.U))
                {
                    ClearAccessLevels();
                    UpdateAccessLevels(0);
                }

                if (Input.GetKeyDown(KeyCode.I))
                {
                    foreach(var device in GetComponentsInChildren<Device>()){
                        device.ExecutePreConnection();
                    }
                }
            }
        }

        public void DeselectSelectedNodes()
        {
            if(selectedNodes != null){
                foreach(var sn in selectedNodes){
                    sn.SetState(NodeState.Off);
                }
            }
            selectedNodes.Clear();
        }

        public void RemoveConnections()
        {
            if(isConnecting){
                isConnecting = false;     
                foreach(var c in connectionBySelectedNode.Values){
                    c.PlayDisconnectionSound();
                    Destroy(c.gameObject);
                }

                connectionBySelectedNode = null; 
            }
        }

        public void DeselectNode(Node node)
        {
            node.SetState(NodeState.Off);

            if(selectedNodes != null){
                selectedNodes.Remove(node);
            }
        }
        
        private void SelectNodes(IEnumerable<Node> nodes)
        {
            foreach (var sn in nodes)
            {
                SelectNode(sn);
            }
        }

        private void SelectNode(Node sn)
        {
            selectedNodes.Add(sn);
            sn.SetState(NodeState.Selected);
        }

        // EVENTS

        internal void OnNodeClickDown(BaseEventData eventData, Node node)
        {
            if (selectedNodes == null || !selectedNodes.Contains(node))
            {
                if(Input.GetKey(KeyCode.LeftControl)){
                    SelectNodeAdd(node);
                } else {
                    SelectNode(node, false);
                }
            } else if (selectedNodes != null){
                if(Input.GetKey(KeyCode.LeftControl)){
                    DeselectNode(node);
                }
            }

            var pointerEvent = (PointerEventData)eventData;
            if (pointerEvent.button == PointerEventData.InputButton.Right)
            {
                if(selectedNodes != null){
                    foreach(var selectedNode in selectedNodes){
                        selectedNode.SelectAllConnectionsForDelete(true);
                    }
                }
            }
        }

        public void SelectNodeAdd(Node node)
        {
            SelectNode(node);
        }

        public void SelectNode(Node node, bool fromHUD = false)
        {
            DeselectSelectedNodes();
            SelectNode(node);
            selectedNodeFromHUD = fromHUD? node : null;
        }

        internal void OnNodeClickUp(BaseEventData eventData, Node node)
        {
            var pointerEvent = (PointerEventData)eventData;
            if (pointerEvent.button == PointerEventData.InputButton.Left)
            {
                StartOrEndConnect(node);
            }

            if (pointerEvent.button == PointerEventData.InputButton.Right)
            {
                PlayDisconnectionSound();
                if(selectedNodes != null){
                    foreach(var selectedNode in selectedNodes){
                        selectedNode.DisconnectAll(true);
                    }
                }
            }
        }

        public void PlayDisconnectionSound()
        {
            disconnectSound.Play(mousePosNode.transform.position);
        }

        internal void OnNodeHoverExit(Node node)
        {
            isConnectionSelectionEnabled = true;
            
            if(selectedNodes != null){
                foreach(var selectedNode in selectedNodes){
                    selectedNode.SelectAllConnectionsForDelete(false);
                }
            }
        }

        internal void OnNodeHoverEnter(Node node)
        {
            isConnectionSelectionEnabled = false;

            if(Input.GetMouseButton(1)){
                if(selectedNodes != null && selectedNodes.Contains(node)){
                    foreach(var selectedNode in selectedNodes){
                        selectedNode.SelectAllConnectionsForDelete(true);
                    }
                }
            }
        }

        public void StartOrEndConnect(Node node)
        {
            node?.SetMoving(false, Vector3.zero);

            if (isConnecting)
            {
                foreach (var selectedNode in connectionBySelectedNode.Keys)
                {
                    var con = connectionBySelectedNode[selectedNode];
                    if(con.reversed){
                        Network.Instance.ConnectionEnd(node, selectedNode, con);
                    } else {
                        Network.Instance.ConnectionEnd(selectedNode, node, con);
                    }
                }
                isConnecting = false;
            }
            else if (!isNodeDragStarted && !Input.GetKey(KeyCode.LeftControl))
            {
                connectionBySelectedNode = new Dictionary<Node, Connection>();
                foreach (var selectedNode in selectedNodes)
                {
                    connectionBySelectedNode.Add(selectedNode, ConnectionStart(selectedNode, true, selectedNode.gate.maxOutputs == 0));
                }
                isConnecting = true;
            }

            foreach (var selectedNode in selectedNodes)
            {
                selectedNode.SetMoving(false, Vector3.zero);
            }

            isNodeDragStarted = false;
            isSelectionDragStarted = false;
        }

        internal void OnNodeBeginDrag(BaseEventData eventData, Node node)
        {
            var pointerEvent = (PointerEventData)eventData;
            if (pointerEvent.button == PointerEventData.InputButton.Left)
            {
                StartNodeDrag();
            }
        }

        public void StartNodeDrag()
        {
            
            isNodeDragStarted = true;
            foreach (var selectedNode in selectedNodes)
            {
                selectedNode.SetMoving(true, selectedNode.transform.position - mousePosNode.transform.position);
            }
        }

        internal void OnNodeDrag(BaseEventData eventData, Node node)
        {
        }

        internal void OnNodeEndDrag(BaseEventData eventData, Node node)
        {
            var pointerEvent = (PointerEventData)eventData;
            if (pointerEvent.button == PointerEventData.InputButton.Left)
            {
                
            }
        }

        public void OnBackgroundClickDown(BaseEventData eventData)
        {
        }
        
        public void OnBackgroundClickUp(BaseEventData eventData)
        {
            var pointerEvent = (PointerEventData)eventData;
            if (pointerEvent.button == PointerEventData.InputButton.Left)
            {
                if(!isSelectionDragStarted){
                    DeselectSelectedNodes();
                }

                foreach (var selectedNode in selectedNodes)
                {
                    selectedNode.SetMoving(false, Vector3.zero);
                }

                isNodeDragStarted = false;
                isSelectionDragStarted = false;
            }

            if (pointerEvent.button == PointerEventData.InputButton.Right)
            {
                RemoveConnections();
            }
        }

        public void OnBackgroundBeginDrag(BaseEventData eventData)
        {
            var pointerEvent = (PointerEventData)eventData;
            if (pointerEvent.button == PointerEventData.InputButton.Left)
            {
                isSelectionDragStarted = true;
                selectionStartPos = mousePosNode.transform.position;
                selectionController.Show();
            }
        }

        public void OnBackgroundDrag(BaseEventData eventData)
        {
            if(isSelectionDragStarted)
            {
                var selectionRect = selectionController.GetSelectionRect(selectionStartPos, mousePosNode.transform.position);
                DeselectSelectedNodes();
                SelectNodes(Physics2D.OverlapBoxAll(selectionRect.position + selectionRect.size / 2, selectionRect.size, 0, nodeLayerMask)
                .Select(x => x.GetComponent<Node>()).Where(x => (x.deviceUI == null || x.deviceUI.selected) && x.accessible));
            }
        }

        public void OnBackgroundEndDrag(BaseEventData eventData)
        {
            selectionController.Hide();
        }
    }
}
