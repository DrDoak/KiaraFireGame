using UnityEngine;
using TMPro;
public class UIDialogHistoryEntry : DialogOutput
{
    [SerializeField]
    private TextMeshProUGUI speakerText;
    [SerializeField]
    private TextMeshProUGUI contentsText;

    public void SetDialog(Dialog dialogEvent)
    {
        if (dialogEvent.dialogSettings.visualOptions.speaker != null)
        {
            speakerText.text = dialogEvent.dialogSettings.visualOptions.speaker.name;
        }
        if (dialogEvent.dialogSettings.visualOptions.overrideSpeakerName != "")
        {
            speakerText.text = dialogEvent.dialogSettings.visualOptions.overrideSpeakerName;
        }
        DialogManager.ForceFullParse(this, dialogEvent);
    }

    public override void AppendText(string text) => contentsText.text += text;
    public override void ClearText() => contentsText.text = "";

    public override void SetName(string newName)
    {
        speakerText.text = newName;
    }
    public override void Show()
    {
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }
}
