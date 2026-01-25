using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class CutsceneChildrenColor : MonoBehaviour
{
    public Color colorToSetTo;
    private List<Image> childImages = new List<Image>();
    private Color lastColor;
    private void Awake()
    {
        childImages = new List<Image>(GetComponentsInChildren<Image>());
        lastColor = colorToSetTo;
    }
    // Update is called once per frame
    void Update()
    {
        if (colorToSetTo != lastColor)
        {
            foreach(Image img in childImages)
            {
                img.color = colorToSetTo;
            }
            lastColor = colorToSetTo;
        }
    }
}
