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
        DialogManager.DialogSignalEvent += RespondToDialogSignal;
    }
    private void OnDestroy()
    {
        DialogManager.DialogSignalEvent -= RespondToDialogSignal;
    }

    private void RespondToDialogSignal(DialogSignal signal)
    {
        if (signal.name.Length < prefix.Length) return;
        if (signal.name.Substring(0, prefix.Length) != prefix) return;
        mAnimator.Play(signal.name.Substring(prefix.Length));
    }
}
