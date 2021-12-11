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
using Lowscope.Saving;

namespace Scripts.Hacking
{
    public class NetworkPersistance : MonoBehaviour, ISaveable
    {
        public Network network;
        private string path;
        private Dictionary<string, Node> levelNodesRepo;
        public GameObject[] nodePrefabs;

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

        public string OnSave()
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

            return JsonUtility.ToJson(saveData);
        }

        public void OnLoad(string data)
        {
            StartCoroutine(DelayedLoad(data));
        }

        private IEnumerator DelayedLoad(string data)
        {
            yield return 0;
            var levelNodes = network.GetComponentsInChildren<Node>();
            levelNodesRepo = new Dictionary<string, Node>();
            foreach (var node in levelNodes)
            {
                node.DisconnectAll(true);

                if(String.IsNullOrEmpty(node.nodeId) || node.nodeId == "FilterDevice_3a427804-df1d-4687-9a3a-f202f8de7443filter"){
                }

                levelNodesRepo.Add(node.nodeId, node);
            }

            var container = JsonUtility.FromJson<SavedGame>(data);
            var savedNodeRepo = container.nodes.ToDictionary(x => x.id);
            var connectedNodes = new HashSet<Tuple<SavedNode, SavedNode>>();

            foreach (var savedNode in container.nodes)
            {
                Connect(savedNode, savedNodeRepo, connectedNodes);
            }
        }

        public bool OnSaveCondition()
        {
            return true;
        }
    }
}
