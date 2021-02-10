using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiSpriteMask : MonoBehaviour
{
    public Transform maskTransform;
    public RectTransform rectTransform;
    public float scaleMultiplier = 3;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        maskTransform.localScale = rectTransform.rect.size * scaleMultiplier;
    }
}
