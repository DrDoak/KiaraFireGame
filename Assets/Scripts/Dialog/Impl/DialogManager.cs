using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Playables;
using Debug = UnityEngine.Debug;

public delegate void RespondToDialogSignal(DialogSignal signal);
public delegate void RespondToDialogStringSignal(string signal);
public class DialogManager : MonoBehaviour, IDialogManager
{
    public Action DialogueSequenceStart;
    public Action DialogueSequenceEnd;
    public static DialogManager Instance { get; private set; } //
    public static DialogSequence CurrentSequence { get { return Instance.currentDialogSequence; } }
    public DialogOutput dialogOutput;

    private PlayableDirector currentlyPausedCutscene;

    public float characterTime = 0.1f;
    private DialogSequence currentDialogSequence;
    [SerializeField]
    private AudioComponent audioComponent;
    public static bool IsDialogPlaying {get { return Instance.dialogPlaying; } }
    private bool dialogPlaying;
    private List<DialogSequence> hasPlayedDialogSequence = new List<DialogSequence>();
    public string defaultTypeSFX;
    private bool isSkippingText = false;
    public static RespondToDialogSignal DialogSignalEvent;
    public static RespondToDialogStringSignal DialogStringSignalEvent;
    public DialogSequence CurrentDialogSequence
    {
        get => currentDialogSequence;
        set
        {
            dialogueSequenceIndex = 0;
            if (currentDialogSequence != null && !hasPlayedDialogSequence.Contains(currentDialogSequence))
            {
                hasPlayedDialogSequence.Add(currentDialogSequence);
            }
            currentDialogSequence = value;
            
            if (currentDialogSequence != null && currentDialogSequence.dialogSequence.Count > 0)
            {
                int newInt = 0;
                CurrentDialog = currentDialogSequence.GetNextDialog(ref newInt);
                dialogueSequenceIndex = newInt;
            }
        }
    }
    private Dialog currentDialog;
    public Dialog CurrentDialog
    {
        get => currentDialog;
        set
        {
            dialogCharacterIndex = 0;
            dialogOutput.ClearText();
            currentDialog = value;
        }
    }

    public bool Finished => dialogCharacterIndex >= CurrentDialog.richText.Length;

    [NotNull] private static readonly Dictionary<string, Func<string,string>> Tags;
    private int dialogCharacterIndex;
    private int dialogueSequenceIndex;

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }
        Instance = this;
#if UNITY_EDITOR
        IDialogManager.OnDialogEvent += Debug.Log;
#endif
    }
    public static void StartSequence(DialogSequence newSequence)
    {
        Instance
            .CurrentDialogSequence = newSequence;
        Instance.Open();
        if (Instance.DialogueSequenceStart != null)
        {
            Instance.DialogueSequenceStart();
        }
    }
    public static void SetSequenceComplete(DialogSequence seq)
    {
        if (!Instance.hasPlayedDialogSequence.Contains(seq))
        {
            Instance.hasPlayedDialogSequence.Add(seq);
        }
    }
    public static void EmitDialogSignal(DialogSignal sig)
    {
        if (DialogSignalEvent != null)
        {
            DialogSignalEvent(sig);
        }
    }
    public static void EmitStringSignal(string id)
    {
        if (DialogStringSignalEvent != null)
        {
            DialogStringSignalEvent(id);
        }
    }
    void ParseNextCharacter(DialogOutput output, Dialog dialog, ref int charIndex)
    {
        while(true){
            if(charIndex >= dialog.richText.Length) return;
            switch (dialog.richText[charIndex++])
            {
                case '\\':
                    output.AppendText(dialog.richText[charIndex] is '<' or '>'
                        ? $"<noparse>{dialog.richText[charIndex]}</noparse>"
                        : $"\\{dialog.richText[charIndex++]}");
                    break;
                case '<':
                    var endIndex = dialog.richText.IndexOf('>', charIndex);
                    var fullTag = dialog.richText[charIndex..endIndex];
                    if (dialog.richText[charIndex] == '/')
                    {
                        output.AppendText($"<{fullTag}>");
                        charIndex = endIndex+1;
                        break;
                    }

                    var spaceIndex = fullTag.IndexOf(' ');
                    spaceIndex = spaceIndex == -1 ? fullTag.IndexOf("=", StringComparison.Ordinal) : spaceIndex;
                    var key = spaceIndex == -1 ? fullTag : fullTag[..spaceIndex];
                    Debug.Assert(Tags.ContainsKey(key));
                    output.AppendText(Tags[key](fullTag));
                    charIndex = endIndex+1;
                    break;
                case ' ':
                    output.AppendText(" ");
                    break;
                default:
                    PlayTypeSound(dialog.richText[charIndex - 1].ToString());
                    output.AppendText(dialog.richText[charIndex - 1].ToString());
                    return;
            }
        }
    }
    
    private void PlayTypeSound(string s)
    {
        if (isSkippingText) return;
        if (dialogCharacterIndex % 2 == 1) return;
        if (s == " " || s == "\t")
        {
            return;
        }
        if (currentDialog != null && currentDialog.dialogSettings.visualOptions.speaker != null &&
            currentDialog.dialogSettings.visualOptions.speaker.replacementTypeSFX != "")
        {
            DialogManager.PlayAudioClip(currentDialog.dialogSettings.visualOptions.speaker.replacementTypeSFX);
        } else if (currentDialog != null && defaultTypeSFX != "")
        {
            PlayAudioClip(defaultTypeSFX);
        }
    }
    public void Open()
    {
        if (CurrentDialog == null) return;
        dialogPlaying = true;
        IDialogManager.InvokeDialogStartEvent();
        CurrentDialog.DialogStartActions();
        if (currentDialog.richText != "") {
            dialogOutput.Show();
        }
        
        if (currentDialog.richText == "" &&
            currentDialog.dialogSettings.cutsceneOptions.dialogPlayOption != DialogSkipOption.HOLD_FOREVER &&
            currentDialog.dialogSettings.cutsceneOptions.dialogPlayOption != DialogSkipOption.AUTO_ADVANCE_ONLY) { Next(); }
    }

    public static bool HasPlayedSequence(DialogSequence seq)
    {
        return (Instance.hasPlayedDialogSequence.Contains(seq));
    }
    public static void SetName(string name)
    {
        Instance.dialogOutput.SetName(name);
    }

    public void Close()
    {
        
        dialogOutput.Hide();
        DialogVisualElements.DialogClose();
        ResumeCutsceneAfterDialog();
        IDialogManager.InvokeDialogEndEvent();
        dialogPlaying = false;
        if (currentDialogSequence != null && !hasPlayedDialogSequence.Contains(currentDialogSequence))
        {
            hasPlayedDialogSequence.Add(currentDialogSequence);
            CurrentDialogSequence = null;
        }
        if (DialogueSequenceEnd != null)
        {
            DialogueSequenceEnd();
        }
    }
    private void UpdateTextboxShow()
    {
        if (currentDialog.richText != "")
        {
            dialogOutput.Show();
        }
        else
        {
            dialogOutput.Hide();
        }
    }
    public void Next(int? option = null)
    {
        Debug.Assert(CurrentDialog != null);
        CurrentDialog.DialogEndActions();
        UpdateTextboxShow();

        if (CurrentDialog.dialogSettings.skipToSequence != null)
        {
            CurrentDialogSequence = CurrentDialog.dialogSettings.skipToSequence;
            UpdateTextboxShow();
            CurrentDialog.DialogStartActions();
            IDialogManager.InvokeDialogNextEvent();
            if (currentDialog.richText == "" && currentDialog.dialogSettings.cutsceneOptions.dialogPlayOption != DialogSkipOption.HOLD_FOREVER && 
                currentDialog.dialogSettings.cutsceneOptions.dialogPlayOption != DialogSkipOption.AUTO_ADVANCE_ONLY) { Next(); }
            return;
        }
        if (CurrentDialog.dialogSettings.skipToSequenceIndex > 0)
        {
            if (CurrentDialog.dialogSettings.skipToSequenceIndex == dialogueSequenceIndex)
            {
                dialogueSequenceIndex++;
                Debug.LogWarning("Potential infinite loop detected in Dialogue Sequence. Breaking out.");
            } else
            {
                dialogueSequenceIndex = CurrentDialog.dialogSettings.skipToSequenceIndex;
            }
        } else
        {
            dialogueSequenceIndex++;
        }
        
        Dialog potentialNextDialog = currentDialogSequence.GetNextDialog(ref dialogueSequenceIndex);
        if (potentialNextDialog != null)
        {
            CurrentDialog = potentialNextDialog;
            UpdateTextboxShow();
            CurrentDialog.DialogStartActions();
            IDialogManager.InvokeDialogNextEvent();
            if (currentDialog.richText == "" &&
                currentDialog.dialogSettings.cutsceneOptions.dialogPlayOption != DialogSkipOption.HOLD_FOREVER && 
                currentDialog.dialogSettings.cutsceneOptions.dialogPlayOption != DialogSkipOption.AUTO_ADVANCE_ONLY) { Next(); }
            return;
        }
        if (option == null)
        {
            if (currentDialogSequence.nextDialog == null)
            {
                Close();
                return;
            }
            Debug.Assert(CurrentDialogSequence.nextDialog != null);
            CurrentDialogSequence = CurrentDialogSequence.nextDialog;
            UpdateTextboxShow();
            CurrentDialog.DialogStartActions();
            IDialogManager.InvokeDialogNextEvent();
            if (currentDialog.richText == "" && currentDialog.dialogSettings.cutsceneOptions.dialogPlayOption != DialogSkipOption.HOLD_FOREVER && 
                currentDialog.dialogSettings.cutsceneOptions.dialogPlayOption != DialogSkipOption.AUTO_ADVANCE_ONLY) { Next(); }
            return;
        }
        Debug.Assert(CurrentDialog.dialogSettings.options != null);
        Debug.Assert(CurrentDialog.dialogSettings.options.Length > option.Value);
        Debug.Assert(0 <= option.Value);
        Debug.Assert(CurrentDialog.dialogSettings.options[option.Value].onSelect != null);
        CurrentDialogSequence = CurrentDialog.dialogSettings.options[option.Value].onSelect;
        IDialogManager.InvokeDialogNextEvent();
        if (currentDialog.richText == "" &&
            currentDialog.dialogSettings.cutsceneOptions.dialogPlayOption != DialogSkipOption.HOLD_FOREVER && 
            currentDialog.dialogSettings.cutsceneOptions.dialogPlayOption != DialogSkipOption.AUTO_ADVANCE_ONLY) { Next(); }
    }
    
    public void PauseCutsceneForDialog(PlayableDirector director)
    {
        currentlyPausedCutscene = director;
        currentlyPausedCutscene.Pause();
    }

    public void ResumeCutsceneAfterDialog()
    {
        currentlyPausedCutscene?.Resume();
        currentlyPausedCutscene = null;
    }

    public bool ForceFinish()
    {
        if (currentDialog.dialogSettings.cutsceneOptions.dialogPlayOption == DialogSkipOption.AUTO_ADVANCE_ONLY ) return false;
        if (currentDialog.dialogSettings.cutsceneOptions.dialogPlayOption == DialogSkipOption.NO_SKIP) return !Finished;
        if (Finished)
            return false;
        isSkippingText = true;
        while (dialogCharacterIndex < CurrentDialog.richText.Length)
            ParseNextCharacter(dialogOutput,CurrentDialog,ref dialogCharacterIndex);
        isSkippingText = false;
        return true;
    }
    public static void ForceFullParse(DialogOutput output, Dialog newForceDialog)
    {
        int tempCharacterIndex = 0;
        Instance.isSkippingText = true;
        while (tempCharacterIndex < newForceDialog.richText.Length)
            Instance.ParseNextCharacter(output, newForceDialog, ref tempCharacterIndex);
        Instance.isSkippingText = false;
    }
    float nextCharacterTime;
    private void Update()
    {
        if(CurrentDialog == null || Finished) return;
        if(nextCharacterTime > Time.unscaledTime) return;
        
        ParseNextCharacter(dialogOutput, CurrentDialog, ref dialogCharacterIndex);
        nextCharacterTime = Time.unscaledTime + characterTime;
    }


    static DialogManager()
    {
        Tags = new Dictionary<string, Func<string,string>>();
        foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                     .SelectMany(x => x.GetTypes())
                     .Where(x => !x.IsAbstract && !x.IsInterface && typeof(IRichTextTag).IsAssignableFrom(x)))
        {
            var richTagHandler = Activator.CreateInstance(type) as IRichTextTag;
            Debug.Assert(richTagHandler != null, nameof(richTagHandler) + " != null");
            foreach (var tag in richTagHandler.Tags)
                if (!Tags.TryAdd(tag, richTagHandler.OnTag))
                    Debug.LogError($"Multiple IRichTextTag trying to handle {tag}, skipping {richTagHandler.GetType().FullName}.");
        }
    }
    internal static void InvokeDialogEvent_internal(string eventName)=>IDialogManager.InvokeDialogEvent(eventName);

    internal static void PlayAudioClip(string eventName)
    {
        Instance.audioComponent.PlaySounds(eventName);
    }

}
