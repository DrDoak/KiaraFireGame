using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogInput : MonoBehaviour
{
    private Transform options;
    private Button[] buttons;
    private TMP_Text[] buttonTexts;
    private int selectedOption;
    private static bool OptionsNext => DialogManager.Instance.CurrentDialog.dialogSettings.options != null &&
                                       DialogManager.Instance.CurrentDialog.dialogSettings.options.Length > 0;
    public void ProcessInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return))
        {
            OnSubmit();
        }
    }
    private void Awake()
    {
        buttons = transform.GetComponentsInChildren<Button>(true);
        buttonTexts = buttons.Select(x=>x.GetComponentInChildren<TMP_Text>(true)).ToArray();
        IDialogManager.OnDialogNext += DialogChange;
        IDialogManager.OnDialogStart += DialogChange;
    }
    private void Update()
    {
        ProcessInput();
    }
    private void OnEnable()
    {
        //InputManager.SetAsDialogMode(this);
    }
    private void OnDisable()
    {
        //InputManager.SetAsDialogMode(null);
    }

    private void OnDestroy()
    {
        IDialogManager.OnDialogNext -= DialogChange;
        IDialogManager.OnDialogStart -= DialogChange;
    }

    private void DialogChange()
    {
        selectedOption = 0;
        Dialog dialog = DialogManager.Instance.CurrentDialog;
        if (!OptionsNext)
        {
            foreach (var button in buttons)
            {
                button.gameObject.SetActive(false);
            }
            return;
        }
        Debug.Assert(dialog.dialogSettings.options != null, "dialog.options != null");
        Debug.Assert(dialog.dialogSettings.options.Length <=buttons.Length);
        for (int i = 0; i < dialog.dialogSettings.options.Length; i++)
        {
            bool active = i < dialog.dialogSettings.options.Length;
            buttons[i].gameObject.SetActive(active);
            if(active)
                buttonTexts[i].text = dialog.dialogSettings.options[i].optionText;
            
        }
        //buttons[0].OnSelect.Invoke();
    }
    public void SelectOption(int index)
    {
        Dialog dialog = DialogManager.Instance.CurrentDialog;
        DialogManager.StartSequence(dialog.dialogSettings.options[index].onSelect);
    }
    public void OnSubmit()
    {
        if (!DialogManager.IsDialogPlaying) return;
        if (DialogManager.Instance.CurrentDialog.dialogSettings.cutsceneOptions.dialogPlayOption == DialogSkipOption.HOLD_FOREVER) return;
        if (DialogManager.Instance.ForceFinish())
            return;

        if (OptionsNext)
        {
            //DialogManager.Instance.Next(selectedOption);
        }
        else if (DialogManager.Instance.CurrentDialog.dialogSettings.cutsceneOptions.dialogPlayOption != DialogSkipOption.AUTO_ADVANCE_ONLY)
        {
            DialogManager.Instance.Next();
        }
    }
}
