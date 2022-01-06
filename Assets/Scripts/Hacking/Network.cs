using System.Collections;
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

        internal bool isConnecting;
        internal HashSet<int> hackedAccessLevels = new HashSet<int>();
        internal DeviceUI lastDeviceMoved;
        internal int baseDeviceSortingOrder;
        internal HashSet<Node> selectedNodes = new HashSet<Node>();
        internal bool isConnectionSelectionEnabled = true;
        
        internal AccessLevelEvent OnUpdateAccessLevel = new AccessLevelEvent();

        private bool isSelectionDragStarted;
        private Vector3 selectionStartPos;
        private bool isNodeDragStarted;
        private Dictionary<Node, Connection> connectionBySelectedNode;
        private Node selectedNodeFromHUD;

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
       
        internal void Connect(Node from, Node to, bool soundOn)
        {
            ConnectionEnd(from, to, ConnectionStart(from, soundOn));
        }

        internal Connection ConnectionStart(Node from, bool soundOn, bool reversed = false)
        {
            var connection = Instantiate(connectionPrefab, transform).GetComponent<Connection>();
            connection.soundOn = soundOn;

            if(reversed){
                connection.start = mousePosNode;
                connection.end = from;
            } else {
                connection.start = from;
                connection.end = mousePosNode;
            }

            connection.reversed = reversed;

            return connection;
        }

        internal void ConnectionEnd(Node from, Node to, Connection connection)
        {
            var connected = from.Connect(to, connection);
            connection.Connected();
            DeselectSelectedNodes();
            if(!connected){
                RemoveConnection(connection);
            }
        }

        private void RemoveConnection(Connection connection)
        {
            connection.PlayDisconnectionSound();
            Destroy(connection.gameObject);
        }

        private void RemoveSelectedNodes()
        {
            if(isConnecting){
                RemoveConnections();
            }

            foreach(var selectedNode in selectedNodes){
                selectedNode.Remove();
            }

            selectedNodes.Clear();
        }

        // Update is called once per frame
        void Update()
        {

            if (selectedNodes != null)
            {
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButton(1))
                {
                    //DeselectSelectedNodes(true, true);
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
                // if (Input.GetKeyDown(KeyCode.U))
                // {
                //     UpdateAccessLevels(100);
                // }

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
            isConnecting = false;     

            foreach(var c in connectionBySelectedNode.Values){
                c.PlayDisconnectionSound();
                Destroy(c.gameObject);
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
                        selectedNode.DisconnectAll();
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
            node.SetMoving(false, Vector3.zero);

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

            if (selectedNodes != null)
            {
                foreach (var selectedNode in selectedNodes)
                {
                    selectedNode.SetMoving(false, Vector3.zero);
                }
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

                isSelectionDragStarted = false;
            }

            if (pointerEvent.button == PointerEventData.InputButton.Right)
            {
                if(isConnecting){
                    RemoveConnections();
                }
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
                .Select(x => x.GetComponent<Node>()).Where(x => x.deviceUI == null || x.deviceUI.selected));
            }
        }

        public void OnBackgroundEndDrag(BaseEventData eventData)
        {
            selectionController.Hide();
        }
    }
}
