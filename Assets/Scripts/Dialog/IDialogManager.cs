using System;

public interface IDialogManager
{
    public void Open();
    public void Close();
    public void Next(int? option = null);
    
    protected static void InvokeDialogEvent(string eventData)=>OnDialogEvent?.Invoke(eventData);
    protected static void InvokeDialogEndEvent()=>OnDialogEnd?.Invoke();
    protected static void InvokeDialogStartEvent()=>OnDialogStart?.Invoke();
    protected static void InvokeDialogNextEvent()=>OnDialogNext?.Invoke();
    

    public static event Action<string> OnDialogEvent;
    public static event Action OnDialogEnd;
    public static event Action OnDialogStart;
    public static event Action OnDialogNext;
}
