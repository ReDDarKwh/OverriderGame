using System.Collections;
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
        private bool isSelectionDragStarted;
        private Vector3 selectionStartPos;
        private bool isDraggingNodesStarted;
        private bool isConnecting;

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
                    //DeselectSelectedNodes(true, true);
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

        public void DeselectSelectedNodes(bool destroyConnection = true, bool makeSound = false)
        {
            // if (destroyConnection)
            // {
            //     if (makeSound)
            //     {
            //         connection.PlayDeconnectedSound();
            //     }
            //     Destroy(connection.gameObject);
            // }
            // connection = null;

            if(selectedNodes != null){
                foreach(var sn in selectedNodes){
                    sn.SetState(NodeState.Off);
                }
            }

            selectedNodes = null;
        }

        public void DeselectNode(Node node)
        {
            node.SetState(NodeState.Off);

            if(selectedNodes != null){
                selectedNodes = selectedNodes.Where(x => x != node).ToList();
            }
        }
        
        private void SelectNodes(IEnumerable<Node> nodes)
        {
            selectedNodes = nodes;
            foreach (var sn in nodes)
            {
                sn.SetState(NodeState.Selected);
            }
        }

        // EVENTS

        internal void OnNodeClickDown(BaseEventData eventData, Node node)
        {
            if(selectedNodes == null || !selectedNodes.Contains(node)){
                DeselectSelectedNodes();
                SelectNodes(new List<Node>(){node});
            }
        }
        
        internal void OnNodeClickUp(BaseEventData eventData, Node node)
        {
            var pointerEvent = (PointerEventData)eventData;
            if (pointerEvent.button == PointerEventData.InputButton.Left)
            {
                node.SetMoving(false, Vector3.zero);

                if(isConnecting){
                    foreach(var selectedNode in selectedNodes){
                        Network.Instance.ConnectionEnd(selectedNode, node);
                    }
                    isConnecting = false;
                }

                foreach(var selectedNode in selectedNodes){
                    selectedNode.SetMoving(false, Vector3.zero);
                }

                if(!isDraggingNodesStarted){
                    DeselectSelectedNodes();
                    SelectNodes(new List<Node>(){node});
                }

                isDraggingNodesStarted = false;
            }
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
            var pointerEvent = (PointerEventData)eventData;
            if (pointerEvent.button == PointerEventData.InputButton.Left)
            {
                
                if(Input.GetKeyDown(KeyCode.LeftControl)){
                    
                    isDraggingNodesStarted = true;
                    foreach(var selectedNode in selectedNodes){
                        selectedNode.SetMoving(true, selectedNode.transform.position - mousePosNode.transform.position);
                    }

                } else {

                    foreach(var selectedNode in selectedNodes){
                        ConnectionStart(selectedNode, true);
                    }

                    isConnecting = true;
                }
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
