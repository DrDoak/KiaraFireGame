using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
public class UIDialogHistory : MonoBehaviour
{
    public static UIDialogHistory Instance { get; private set; }

    [SerializeField]
    private Animator mAnimator;
    [SerializeField]
    private Transform dialogArea;
    [SerializeField]
    private GameObject DialogItemPrefab;
    [SerializeField]
    private RectTransform scrollArea;
    [SerializeField]
    private float minSize = 1200f;
    [SerializeField]
    private float itemSize = 100f;
    [SerializeField]
    private ScrollRect scrollRect;

    private List<UIDialogHistoryEntry> myItems = new List<UIDialogHistoryEntry>();
    public bool IsCurrentlyShowing { get { return isCurrentlyShowing; } }
    private bool isCurrentlyShowing;
    void Start()
    {
        if (Instance != null) return;
        UIDialogHistory.Instance = this;
    }
    public void NewDialogEvent(Dialog newDialog)
    {
        if (newDialog.richText.Length == 0) return;
        if (myItems.Count > 50) { Destroy(myItems[0]); }
        GameObject newUIElement = Instantiate(DialogItemPrefab, dialogArea);
        newUIElement.GetComponent<UIDialogHistoryEntry>().SetDialog(newDialog);
        myItems.Add(newUIElement.GetComponent<UIDialogHistoryEntry>());
        scrollArea.sizeDelta = new Vector2(scrollArea.sizeDelta.x, Mathf.Max(minSize, (myItems.Count + 1) * itemSize));
    }

    public void ToggleInventory()
    {
        if (isCurrentlyShowing)
        {
            OnClose();
        }
        else
        {
            OnShow();
        }
    }
    public void OnShow()
    {
        if (!isCurrentlyShowing)
        {
            DialogManager.PlayAudioClip("Panel-open");
            mAnimator.Play("show");
        }
        isCurrentlyShowing = true;
        //if (UIInventory.Instance.IsCurrentlyShowing)
        //{
        //    UIInventory.Instance.ToggleInventory();
        //}
        scrollRect.normalizedPosition = new Vector2(0, 0);
    }
    public void OnClose()
    {
        if (isCurrentlyShowing)
        {
            DialogManager.PlayAudioClip("Panel-open");
            mAnimator.Play("hide");
        }
        isCurrentlyShowing = false;
    }
}
