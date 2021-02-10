
using UnityEngine;

namespace Scripts.Gadgets.Modules
{
    public class Placable : GadgetModule
    {
        public GameObject prefab;
        public GameObject preview;
        public float range;
        public LayerMask gadgetLayer;
        internal Transform mousePos;

        void Start()
        {
            mousePos = GameObject.FindGameObjectWithTag("MousePos").transform;
        }

        void Update()
        {
            var centerToMouse = mousePos.position - transform.position;
            preview.transform.position = transform.position + Vector3.ClampMagnitude(centerToMouse, range);

            var collider = Physics2D.OverlapPoint(preview.transform.position, gadgetLayer);

            AttachedGadgetController attachedGadgetController = null;
            if (collider != null)
            {
                attachedGadgetController = collider.GetComponent<AttachedGadgetController>();
                attachedGadgetController.selection.enabled = true;
            }

            if (Input.GetMouseButtonDown(0) && centerToMouse.magnitude < range)
            {
                if (collider != null)
                {
                    attachedGadgetController.AttachGadget(prefab);
                }
                else
                {
                    Instantiate(prefab, preview.transform.position, Quaternion.identity);
                }

                repo.SelectGadget(null);
            }
        }

        void OnEnable()
        {

        }

        void OnDisable()
        {

        }
    }
}