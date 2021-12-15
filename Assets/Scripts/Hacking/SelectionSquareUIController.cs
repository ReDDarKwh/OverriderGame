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

        void Start(){
            line = new VectorLine("SelectionLine", new List<Vector3>(5), lineWidth, lineDepth);
            line.joins = Joins.Weld;
            line.lineType = LineType.Continuous;
            line.maxWeldDistance = lineMaxWeldDistance;
            line.material = lineMaterial;
            line.color = lineColor;
        }

        void Update(){

            line.SetWidth(lineWidth / Camera.main.orthographicSize);

            if (Input.GetMouseButtonDown(0))
            {
                selectingTargetsStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButton(0))
            {
                var mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var width = mousePosWorld.x - selectingTargetsStartPos.x;
                var height = mousePosWorld.y - selectingTargetsStartPos.y;
                var pos = new Vector3(width > 0 ? selectingTargetsStartPos.x : mousePosWorld.x, height > 0 ? selectingTargetsStartPos.y : mousePosWorld.y);
                DrawSelectionSquare(width, height, pos);
            }

            if (Input.GetMouseButtonUp(0))
            {
                DrawSelectionSquare(0, 0, Vector3.zero);
            }

            line.Draw3D(); 
        }

        private void DrawSelectionSquare(float width, float height, Vector3 pos)
        {
            selectionBG.position = pos + new Vector3(Mathf.Abs(width) / 2, Mathf.Abs(height) / 2);
            selectionBG.localScale = new Vector3(width, height);

            DrawBox(line, pos, new Vector2(Mathf.Abs(width), Mathf.Abs(height)));
        }

        void DrawBox(VectorLine line, Vector2 point, Vector2 size)
        {
            line.MakeRect(new Rect(point, size));
        }

    }
}
