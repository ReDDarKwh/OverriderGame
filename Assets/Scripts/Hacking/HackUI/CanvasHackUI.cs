using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Scripts.Hacking
{
    public class CanvasHackUI : HackUI
    {
        public Canvas canvas;
        public List<GameObject> toDisable;

        public override void Hide()
        {
            canvas.enabled = false;

            foreach (var g in toDisable)
            {
                if(g)
                    g.SetActive(false);
            }

            // foreach (var node in GetNodeHackUIs())
            // {
            //     node.Hide();
            // }
        }

        public override void Show()
        {
            canvas.enabled = true;

            foreach (var g in toDisable)
            {
                if(g)
                    g.SetActive(true);
            }

            // foreach (var node in GetNodeHackUIs())
            // {
            //     node.Show();
            // }
        }

        private NodeHackUI[] GetNodeHackUIs()
        {
            return GetComponentsInChildren<NodeHackUI>();
        }

    }
}
