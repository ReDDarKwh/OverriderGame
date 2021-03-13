using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Actions;
using UnityEngine;

namespace Scripts.Hacking
{
    public class Network : MonoBehaviour
    {
        public Connection connection;
        public GameObject connectionPrefab;
        public Node mousePosNode;
        public static Network Instance = null;

        internal int accessLevel;
        internal DeviceUI lastDeviceMoved;
        internal int baseDeviceSortingOrder;
        internal Node selectedNode;

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

        public void DeselectSelectedNode(bool destroyConnection = true)
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

        internal void ConnectionStart(Node from)
        {
            connection = Instantiate(connectionPrefab, transform).GetComponent<Connection>();
            connection.start = from;
            connection.end = mousePosNode;
        }

        internal void ConnectionEnd(Node from, Node to)
        {
            var connected = from.Connect(to, connection);
            DeselectSelectedNode(!connected);
        }

        // Update is called once per frame
        void Update()
        {
            if (selectedNode != null)
            {
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButton(1))
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
        }
    }
}
