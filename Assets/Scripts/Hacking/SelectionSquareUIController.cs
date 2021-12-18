using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Actions;
using UnityEngine;
using Vectrosity;

namespace Scripts.Hacking
{
    public class SelectionSquareUIController : MonoBehaviour
    {
        public Color lineColor;
        public float lineWidth;
        public int lineDepth;
        public float lineMaxWeldDistance;
        public Material lineMaterial;
        public Transform selectionBG;

        private VectorLine line;
        private Vector3 selectingTargetsStartPos;
        
        private float width;
        private float height;
        private Vector3 pos;

        public Rect SelectionRect
        {
            get => new Rect(pos, new Vector2(Mathf.Abs(width), Mathf.Abs(height)));
        }

        private bool isSelecting;
        public LayerMask nodeLayerMask;
        internal IEnumerable<Node> selectedNodes;

        void Start(){
            line = new VectorLine("SelectionLine", new List<Vector3>(5), lineWidth, lineDepth);
            line.joins = Joins.Weld;
            line.lineType = LineType.Continuous;
            line.maxWeldDistance = lineMaxWeldDistance;
            line.material = lineMaterial;
            line.color = lineColor;
        }

        void Update(){


        }

        
        private void DrawSelectionSquare(float width, float height, Vector3 pos)
        {
            selectionBG.position = pos + new Vector3(Mathf.Abs(width) / 2, Mathf.Abs(height) / 2);
            selectionBG.localScale = new Vector3(width, height);
            line.MakeRect(new Rect(pos, new Vector2(Mathf.Abs(width), Mathf.Abs(height))));
        }
      
    }
}
