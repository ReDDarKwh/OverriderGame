using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveUI : MonoBehaviour
{
    public Image checkboxImage;
    public TextMeshProUGUI objectiveText;
    internal string objectiveName;

    // Start is called before the first frame update
    void Start()
    {
        objectiveText.SetText(objectiveName);
        checkboxImage.enabled = false;
    }

    public void ObjectiveDone()
    {
        checkboxImage.enabled = true;
    }
}
