using System;
using JetBrains.Annotations;
using UnityEngine;
using System.Collections.Generic;
using FMODUnity;

[Serializable]
public class Dialog
{
    [TextArea]
    public string richText;
    [CanBeNull] 
    public ExtraDialogOptions dialogSettings;
    public bool CanDisplay()
    {
        return dialogSettings.conditionsToDisplayDialogue.ConditionsActive();
    }
    
    public void DialogStartActions()
    {
        ProcessName();
        dialogSettings.actionsOnStartDialog.TriggerActions();
        DialogVisualElements.ProcessVisualElements(dialogSettings.visualOptions);
        DialogVisualElements.ProcessCutscene(dialogSettings.cutsceneOptions);
        if (UIDialogHistory.Instance != null)
        {
            UIDialogHistory.Instance.NewDialogEvent(this);
        }
    }
    public void DialogEndActions()
    {
        dialogSettings.actionsAfterDialogClose.TriggerActions();
    }
    
    private void ProcessName()
    {
        DialogManager.SetName("");
        if (dialogSettings.visualOptions.speaker != null)
        {
            DialogManager.SetName(dialogSettings.visualOptions.speaker.name);
            return;
        }
        if (dialogSettings.visualOptions.overrideSpeakerName != "")
        {
            DialogManager.SetName(dialogSettings.visualOptions.overrideSpeakerName);
            return;
        }
    }
}
[Serializable]
public class ExtraDialogOptions
{
    public DialogOption[] options;
    public DialogElements visualOptions;
    public DialogConditions conditionsToDisplayDialogue;
    public DialogCutsceneSettings cutsceneOptions;
    public DialogActions actionsOnStartDialog;
    public DialogActions actionsAfterDialogClose;
    public DialogSequence skipToSequence;
    public int skipToSequenceIndex;
}

public enum DialogSpeakerPosition { DEFAULT, LEFT_SPEAKER, RIGHT_SPEAKER };

[Serializable]
public class DialogElements
{
    public bool clearAllPreviousPortraits = true;
    public SpeakerInfo speaker;
    [Header("Speaker Extra Options")]
    public string overrideSpeakerName;
    public DialogSpeakerPosition overrideSpeakerPosition;
    public Sprite overridePortrait;
    public DialogSpeakerAnimation playSpeakerAnimation;
    public DialogExpression expression;
}
public enum DialogSkipOption { STANDARD, NO_SKIP, AUTO_ADVANCE_ONLY, HOLD_FOREVER}
[Serializable]
public class DialogCutsceneSettings
{
    [Header("Cutscene Options")]
    public DialogSkipOption dialogPlayOption = DialogSkipOption.STANDARD;
    public Sprite showImage;
    public GameObject showObject;
    public string playAnimation;
    public float forcePause = 0.0f;
    public EventReference startMusicEvent;
    public bool stopMusic;
    public bool addExtraLayer;
    public EventReference ambienceEvent;
    public bool stopAmbience;
}
[Serializable]
public class DialogConditions
{
    public string requiredKeyToPlay = "";
    public string prohibitedKeyToPlay = "";
    public List<DialogSequence> requiredSequencesComplete = new List<DialogSequence>();
    public List<DialogSequence> prohibitedSequencesComplete = new List<DialogSequence>();
    
    public bool ConditionsActive()
    {
        //if (requiredKeyToPlay != "" &&
        //    !SaveManager.IsFlagSet(requiredKeyToPlay))
        //    return false;
        //if (prohibitedKeyToPlay != "" &&
        //    SaveManager.IsFlagSet(prohibitedKeyToPlay))
        //    return false;
        foreach (DialogSequence item in requiredSequencesComplete)
        {
            if (!DialogManager.HasPlayedSequence(item))
            {
                return false;
            }
        }
        foreach (DialogSequence item in prohibitedSequencesComplete)
        {
            if (DialogManager.HasPlayedSequence(item))
            {
                return false;
            }
        }
        return true;
    }

}
[Serializable]
public struct DialogOption
{
    public string optionText;
    [SerializeField]
    public DialogSequence onSelect;
    //TODO: condition to show up?
    public DialogConditions conditionsToDisplayDialogue;
    public bool ShouldDisplayOption()
    {
        return conditionsToDisplayDialogue.ConditionsActive();
    }
}

[Serializable]
public class DialogActions
{
    public DialogSignal emitSignal;
    public string emitStringSignal;
    public string playSFX;
    //public WorldData loadWorld;

    public void TriggerActions()
    {
        if (emitSignal != null)
        {
            DialogManager.EmitDialogSignal(emitSignal);
        }
        if (emitStringSignal != "")
        {
            DialogManager.EmitStringSignal(emitStringSignal);
        }

        if (playSFX != "")
        {
            DialogManager.PlayAudioClip(playSFX);
        }
        //if (loadWorld != null)
        //{
        //    WorldManager.LoadNewWorld(loadWorld);
        //}
    }
}
