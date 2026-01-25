using UnityEngine;

public abstract class DialogOutput : MonoBehaviour
{
    public abstract void AppendText(string text);
    public abstract void ClearText();

    public abstract void SetName(string newName);
    public abstract void Show();
    public abstract void Hide();

}
