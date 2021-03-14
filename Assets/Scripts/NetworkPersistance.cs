using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Xml.Serialization;
using System.IO;
using System;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using Scripts.Actions;

namespace Scripts.Hacking
{
    public class NetworkPersistance : MonoBehaviour
    {
        public Network network;
        private string path;
        private Dictionary<string, Node> levelNodesRepo = new Dictionary<string, Node>();
        public GameObject[] nodePrefabs;

        void Start()
        {
            StartCoroutine(DelayedLoad());
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                Save();
            }
        }

        public void RunPreConnections()
        {
            foreach (var device in GetComponentsInChildren<Device>())
            {
                device.ExecutePreConnection();
            }
        }

        public IEnumerator DelayedLoad()
        {
            yield return new WaitForSeconds(0.5f);
            path = Application.dataPath + $"/LevelData/{SceneManager.GetActiveScene().name}.json";
            var levelNodes = network.GetComponentsInChildren<Node>();
            foreach (var node in levelNodes)
            {
                levelNodesRepo.Add(node.nodeId, node);
            }

            yield return new WaitForSeconds(0.5f);
            Load();
        }

        public void Save()
        {
            var nodes = network.GetComponentsInChildren<Node>();
            var savedNodes = nodes.Select(x =>
            {
                return new SavedNode
                {
                    id = x.nodeId,
                    gateType = (int)x.gateType,
                    pos = new float[] { x.transform.position.x, x.transform.position.y },
                    connections = x
                    .gate
                    .children
                    .Where(c => c.node != null)
                    .Select(c =>
                    {
                        return c.node.nodeId;
                    })
                    .ToArray(),
                    data = GetGateData(x.gate)
                };
            });

            var saveData = new SavedGame { nodes = savedNodes.ToArray() };

            using (StreamWriter file = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, saveData);
            }
        }

        private object GetGateData(AbstractGate x)
        {
            if (x is DataGate)
            {
                var dataGate = (DataGate)x;

                if (dataGate.saveData)
                {
                    return dataGate.GetCurrentData();
                }
            }
            return null;
        }

        public void Load()
        {
            using (StreamReader file = File.OpenText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                var container = (SavedGame)serializer.Deserialize(file, typeof(SavedGame));
                var savedNodeRepo = container.nodes.ToDictionary(x => x.id);
                var connectedNodes = new HashSet<Tuple<SavedNode, SavedNode>>();

                foreach (var savedNode in container.nodes)
                {
                    Connect(savedNode, savedNodeRepo, connectedNodes);
                }
            }
        }

        private Node Connect(SavedNode savedNode, Dictionary<string, SavedNode> savedNodeRepo, HashSet<Tuple<SavedNode, SavedNode>> connectedNodes)
        {
            Node node = null;
            if (levelNodesRepo.ContainsKey(savedNode.id))
            {
                node = levelNodesRepo[savedNode.id];
            }
            else
            {
                if (savedNode.gateType != (int)GateType.DATA)
                {
                    node = Instantiate(
                        nodePrefabs[savedNode.gateType],
                        new Vector3(savedNode.pos[0], savedNode.pos[1]),
                        Quaternion.identity,
                        transform
                    ).GetComponent<Node>();
                    node.Init(null, savedNode.id);
                    levelNodesRepo.Add(savedNode.id, node);
                }
            }

            if (node == null)
            {
                return null;
            }

            if (node.gate is DataGate && savedNode.data != null)
            {
                ((DataGate)node.gate).SetData(savedNode.data);
            }

            foreach (var childId in savedNode.connections)
            {
                var childSavedNode = savedNodeRepo[childId];
                var c = new Tuple<SavedNode, SavedNode>(savedNode, childSavedNode);

                if (!connectedNodes.Contains(c))
                {
                    connectedNodes.Add(c);
                    network.Connect(node, Connect(childSavedNode, savedNodeRepo, connectedNodes), false);
                }
            }

            return node;
        }
    }
}
