using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Scripts.Hacking;
using UnityEngine;
using Network = Scripts.Hacking.Network;

namespace Scripts.UI 
{
    public class SecurityRoomUIController : UnityEngine.MonoBehaviour
    {
        public int accessLevelId;
        public Image accessLevelDotImage;
        // Start is called before the first frame update
        void Start()
        {
            accessLevelDotImage.color = Network.Instance.accessLevels[accessLevelId];
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetButtonDown("HideHacking")){
                accessLevelDotImage.enabled = !accessLevelDotImage.enabled;
            }
        }
    }
}
