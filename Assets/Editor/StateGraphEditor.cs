using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Scripts.StateMachine;
using Scripts.StateMachine.Graph;

namespace XNodeEditor.Custom
{
    [CustomNodeGraphEditor(typeof(SMGraph))]
    class StateGraphEditor : NodeGraphEditor
    {
        /// <summary> Create a node and save it in the graph asset </summary>
        private void CreateNode(XNode.Node node, string name, Vector2 position)
        {
            node.position = position;
            node.name = name;

            CopyNode(node);
        }

        public override void OnDropObjects(UnityEngine.Object[] objects)
        {

            Vector2 pos = NodeEditorWindow.current.WindowToGridPosition(Event.current.mousePosition);
            foreach (var droppedObject in objects)
            {

                if (droppedObject.name.EndsWith("State"))
                {
                    var node = ScriptableObject.CreateInstance(typeof(StateNode)) as StateNode;
                    var state = droppedObject.name;
                    node.states = new List<string> { state };
                    CreateNode(node, state, pos);
                }
                else if (droppedObject.name.EndsWith("Event"))
                {
                    var node = ScriptableObject.CreateInstance(typeof(LinkNode)) as LinkNode;
                    var evt = droppedObject.name;
                    node.trigger = evt;
                    node.eventName = evt;
                    CreateNode(node, evt, pos);
                }
                else if (droppedObject is SMGraph)
                {
                    var node = ScriptableObject.CreateInstance(typeof(StateNode)) as StateNode;

                    var graph = droppedObject as SMGraph;

                    node.states = graph.nodes.Where(x => x is StateNode)
                    .SelectMany(x => (x as StateNode).states).Distinct().ToList();
                    CreateNode(node, graph.name, pos);
                }
            }
        }
    }
}

