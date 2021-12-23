using System;
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
        public Transform mousePos;
        private VectorLine line;
        private bool isVisible;
        private Vector3 selectionStartPos;

        void Start(){
            line = new VectorLine("SelectionLine", new List<Vector3>(5), lineWidth, lineDepth);
            line.joins = Joins.Weld;
            line.lineType = LineType.Continuous;
            line.maxWeldDistance = lineMaxWeldDistance;
            line.material = lineMaterial;
            line.color = lineColor;
        }

        void Update(){
            if(isVisible){
                DrawSelectionSquare(selectionStartPos, mousePos.position);
                line.Draw3D();
            }
        }

        public Rect GetSelectionRect(Vector3 start, Vector3 end)
        {
            var width = end.x - start.x;
            var height = end.y - start.y;
            var pos = new Vector3(width > 0 ? start.x : end.x, height > 0 ? start.y : end.y);
            return new Rect(pos, new Vector2(Mathf.Abs(width), Mathf.Abs(height)));
        }
        
        private void DrawSelectionSquare(Vector3 start, Vector3 end)
        {
            var rect = GetSelectionRect(start, end);
            selectionBG.position = (Vector3)(rect.position) + new Vector3(Mathf.Abs(rect.width) / 2, Mathf.Abs(rect.height) / 2);
            selectionBG.localScale = new Vector3(rect.width, rect.height);
            line.MakeRect(rect);
        }

        internal void Show()
        {
            isVisible = true;
            selectionStartPos = mousePos.position;
        }

        internal void Hide()
        {
            isVisible = false;
            DrawSelectionSquare(Vector3.zero, Vector3.zero);
            line.Draw3D();
        }
    }
}
