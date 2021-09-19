using System.Collections;
using System.Collections.Generic;
using Lowscope.Saving;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SaveMaster.SetSlot(0, false);
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.F5))
        // {
        //     SaveMaster.SyncSave();
        //     SaveMaster.WriteActiveSaveToDisk();
        // }

        // if (Input.GetKeyDown(KeyCode.F9))
        // {
        //     SaveMaster.SyncLoad();
        // }
    }
}
