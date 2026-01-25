using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueAnimationResponder : MonoBehaviour
{
    [SerializeField]
    private string prefix;
    [SerializeField]
    private Animator mAnimator;
    // Start is called before the first frame update
    void Start()
    {
        if (DialogManager.Instance == null) return;
        DialogManager.DialogStringSignalEvent += RespondToDialogStringSignal;
    }
    private void OnDestroy()
    {
        DialogManager.DialogStringSignalEvent -= RespondToDialogStringSignal;
    }

    private void RespondToDialogStringSignal(string signal)
    {
        if (signal.Length < prefix.Length) return;
        if (signal.Substring(0, prefix.Length) != prefix) return;
        mAnimator.Play(signal.Substring(prefix.Length));
    }
}
