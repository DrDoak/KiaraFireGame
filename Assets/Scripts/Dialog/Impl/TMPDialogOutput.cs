using TMPro;
using UnityEngine;

public class TMPDialogOutput : DialogOutput
{
    [SerializeField]
    TextMeshProUGUI textMesh;
    [SerializeField]
    TextMeshProUGUI nameMesh;

#if UNITY_EDITOR
    void OnValidate()
    {
        if(textMesh == null)
            textMesh = GetComponentInChildren<TextMeshProUGUI>();
        if (textMesh == null) return;
        textMesh.richText = true;
    }
    #endif
    
    public override void AppendText(string text) => textMesh.text += text;
    public override void ClearText() => textMesh.text = "";
    
    //TODO
    public override void Show()
    {
        gameObject.SetActive(true);
    }

    public override void SetName(string newName)
    {
        nameMesh.text = newName;
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }
}
