using System.Collections.Generic;
using UnityEngine;

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
                g.SetActive(false);
            }
        }

        public override void Show()
        {
            canvas.enabled = true;

            foreach (var g in toDisable)
            {
                g.SetActive(true);
            }
        }

    }
}
