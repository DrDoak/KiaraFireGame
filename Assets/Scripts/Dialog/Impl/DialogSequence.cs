using UnityEngine;
using JetBrains.Annotations;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "dialogSequence", menuName = "Holojam/DialogSequence", order = 0)]
public class DialogSequence : ScriptableObject
{
    [SerializeField]
    public List<Dialog> dialogSequence;
    [CanBeNull] public DialogSequence nextDialog;

    public Dialog GetNextDialog(ref int nextInSequence)
    {
        if (nextInSequence >= dialogSequence.Count) return null;
        Dialog nextDialog = null;
        do
        {
            if (nextInSequence >= dialogSequence.Count) break;
            Dialog potentialDialog = dialogSequence[nextInSequence];
            if (potentialDialog.CanDisplay())
            {
                nextDialog = potentialDialog;
                break;
            }
            nextInSequence++;
        }
        while (nextDialog == null);
        return nextDialog;
    }
}
