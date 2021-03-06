using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Actions;
using UnityEngine;

namespace Scripts.Hacking
{
    public class Network : MonoBehaviour
    {
        public Node selectedNode;
        public float interactionRadius;
        public Transform mousePos;
        public Connection connection;
        public GameObject connectionPrefab;
        public float nodeSearchRadius;
        public LayerMask nodeLayerMask;
        public Node mousePosNode;
        public static Network Instance = null;

        internal int accessLevel;
        private Node[] nodes;

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

        void DeselectSelectedNode(bool destroyConnection = true)
        {
            selectedNode = null;
            if (destroyConnection)
            {
                Destroy(connection.gameObject);
            }
            connection = null;
        }

        internal void Connect(Node from, Node to)
        {
            ConnectionStart(from);
            ConnectionEnd(from, to);
        }

        private void ConnectionStart(Node from)
        {
            connection = Instantiate(connectionPrefab, transform).GetComponent<Connection>();
            connection.start = from;
            connection.end = mousePosNode;
        }

        private void ConnectionEnd(Node from, Node to)
        {
            var connected = from.Connect(to, connection);
            DeselectSelectedNode(!connected);
        }

        // Update is called once per frame
        void Update()
        {
            float minDis = float.MaxValue;
            Node closestNode = null;
            Collider2D closestCollider = null;
            var hits = Physics2D.OverlapCircleAll(mousePos.position, nodeSearchRadius, nodeLayerMask);
            foreach (var hit in hits)
            {
                var dis = (mousePos.position - hit.transform.position).magnitude;
                if (dis < minDis)
                {
                    closestCollider = hit;
                    minDis = dis;
                }
            }

            if (closestCollider != null)
            {
                closestNode = closestCollider.GetComponent<Node>();
            }

            if (closestNode != null)
            {
                if ((mousePos.position - closestNode.transform.position).magnitude < interactionRadius)
                {
                    if (closestNode != selectedNode && closestNode.isVisible && closestNode.accessible)
                    {
                        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && !closestNode.moving && !Input.GetKey(KeyCode.LeftControl))
                        {
                            if (selectedNode == null)
                            {
                                if (Input.GetMouseButtonDown(1))
                                {
                                    closestNode.gate.SetValue(!closestNode.gate.currentValue);
                                }
                                else
                                {
                                    if (closestNode.maxOutputs > 0)
                                    {
                                        selectedNode = closestNode;
                                        ConnectionStart(closestNode);
                                    }
                                }
                            }
                            else
                            {
                                if (Input.GetMouseButtonDown(1))
                                {
                                    selectedNode.Disconnect(closestNode);
                                    DeselectSelectedNode();
                                }
                                else
                                {
                                    ConnectionEnd(selectedNode, closestNode);
                                }
                            }
                        }
                    }
                }
            }

            if (selectedNode != null)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    DeselectSelectedNode();
                }

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    selectedNode.gate.SetValue(!selectedNode.gate.currentValue);
                }

                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    selectedNode.Remove();
                    DeselectSelectedNode();
                }
            }

            // if (Input.GetKeyDown(KeyCode.E))
            // {
            //     uiVisible = !uiVisible;

            //     if (uiVisible)
            //     {
            //         ShowHUD();
            //     }
            //     else
            //     {
            //         HideHUD();
            //     }
            // }
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
    }
}
