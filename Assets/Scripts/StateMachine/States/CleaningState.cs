using Bolt;
using Scripts.Actions;
using UnityEngine;

namespace Scripts.States
{
    public class CleaningState : AbstractState
    {
        public ExternalLogicAction cleaningAction;
        public float cleanSpeed;
        public LayerMask dirtLayer;
        public Animator animator;

        private GameObject target;
        private bool isCleaningDirt;
        private SpriteRenderer spriteRenderer;
        private float alpha;
        private Color baseColor;

        public override void StateEnter()
        {
            target = Variables.Object(gameObject).Get<GameObject>("target");
            isCleaningDirt = dirtLayer == (dirtLayer | 1 << target.layer);
            animator.SetBool("Washing", true);

            if (isCleaningDirt)
            {
                spriteRenderer = target.GetComponent<SpriteRenderer>();
                alpha = spriteRenderer.color.a;
                baseColor = spriteRenderer.color;
            }
            else
            {
                alpha = 1;
            }
        }

        public override void StateUpdate()
        {
            cleaningAction.actionGate.SetValue(true);
            alpha = Mathf.Clamp(alpha - cleanSpeed * Time.deltaTime, 0, 1);

            if (isCleaningDirt)
            {
                spriteRenderer.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            }

            if (alpha == 0)
            {
                if (isCleaningDirt)
                {
                    target.layer = 0;
                    Destroy(target, 5);
                }

                cleaningAction.actionGate.SetValue(false);
                CustomEvent.Trigger(gameObject, "CleanDone");
            }
        }

        public override void StateExit()
        {
            animator.SetBool("Washing", false);
        }
    }
}