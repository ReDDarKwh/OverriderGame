using System.Collections;
using System.Collections.Generic;
using Scripts.StateMachine.Graph;
using UnityEngine;

namespace XNodeEditor.Examples
{
    [CustomNodeGraphEditor(typeof(StateGraph))]
    public class StateGraphEditor : NodeGraphEditor
    {

        /// <summary> 
        /// Overriding GetNodeMenuName lets you control if and how nodes are categorized.
        /// In this example we are sorting out all node types that are not in the XNode.Examples namespace.
        /// </summary>
        public override string GetNodeMenuName(System.Type type)
        {
            if (type.Namespace == "XNode.Examples.StateGraph")
            {
                return base.GetNodeMenuName(type).Replace("X Node/Examples/State Graph/", "");
            }
            else return null;
        }

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

                // if (droppedObject.name.EndsWith("State"))
                // {
                //     var node = ScriptableObject.CreateInstance(typeof(StateNode)) as StateNode;
                //     var state = droppedObject.name;
                //     node.state = state;
                //     CreateNode(node, state, pos);
                // }
                if (droppedObject.name.EndsWith("Event"))
                {
                    var node = ScriptableObject.CreateInstance(typeof(LinkNode)) as LinkNode;
                    var evt = droppedObject.name;
                    node.trigger = evt;
                    node.triggerName = evt;
                    CreateNode(node, evt, pos);
                }
                // else if (droppedObject is SMGraph)
                // {
                //     var node = ScriptableObject.CreateInstance(typeof(StateGroupNode)) as StateGroupNode;
                //     var graph = droppedObject as SMGraph;
                //     node.stateMachineGraph = graph;
                //     CreateNode(node, graph.name, pos);
                // }
            }
        }
    }
}