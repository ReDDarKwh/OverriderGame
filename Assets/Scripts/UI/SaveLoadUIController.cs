using System.Collections;
using System.Collections.Generic;
using Lowscope.Saving;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadUIController : MonoBehaviour
{
    public GameObject gameSavePrefab;
    public Transform gameSaveContainer;
    public GameObject ConfirmDialog;
    public SceneChanger sceneChanger;
    
    private GameSaveUIController selectedSlot;

    // Start is called before the first frame update
    void Start()
    {
        for(var i = 1; i < 8 * 2 - 1; i += 2){
            var gs = Instantiate(gameSavePrefab, gameSaveContainer).GetComponent<GameSaveUIController>();
            gs.slotNumber = i;
            gs.saveLoadUIController = this;
            
            UpdateDisplay(gs);
        }
    }

    void UpdateDisplay(GameSaveUIController gs){
         if(SaveMaster.IsSlotUsed(gs.slotNumber)){
                gs.text.text = "Slot " + ((gs.slotNumber + 1) / 2) + " - " + SaveMaster.GetSaveCreationTime(gs.slotNumber);
            } else {
                gs.text.text = "Empty";
            }
    }

    public void SelectSaveSlot(GameSaveUIController gs)
    {
        selectedSlot = gs;
    }

    void Save(){
        SaveMaster.DeleteSave(selectedSlot.slotNumber);
        SaveMaster.SetSlot(selectedSlot.slotNumber, false);
        SaveMaster.WriteActiveSaveToDisk();

        UpdateDisplay(selectedSlot);
    }

    public void Load(){
        SaveMaster.ClearActiveSavedPrefabs();
        SaveMaster.SetSlot(selectedSlot.slotNumber, false);
        SaveMaster.SyncLoad();
    }

    public void Exit(){
        sceneChanger.RemoveSceneAdditive("");
    }

    public void ConfirmSaveLoad(){
        Save();
        HideConfirm();
    }

    public void CancelSaveLoad(){
        HideConfirm();
    }

    public void ShowConfirm(){
        this.ConfirmDialog.SetActive(true);
    } 

    void HideConfirm(){
        this.ConfirmDialog.SetActive(false);
    }
}
