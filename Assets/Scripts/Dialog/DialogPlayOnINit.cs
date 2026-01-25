using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogPlayOnINit : MonoBehaviour
{
    public DialogSequence playOnInit;
    public DialogSequence specialEndingDialog;
    public DialogSequence playOnDestroy;

    // Start is called before the first frame update
    void Start()
    {
        if (specialEndingDialog != null)
        {
            if (PlayerPrefs.HasKey("Stage1-all-enemies") && PlayerPrefs.GetInt("Stage1-all-enemies") == 1 &&
                PlayerPrefs.HasKey("Stage2-all-enemies") && PlayerPrefs.GetInt("Stage2-all-enemies") == 1 &&
                PlayerPrefs.HasKey("Stage3-all-enemies") && PlayerPrefs.GetInt("Stage3-all-enemies") == 1 &&
                PlayerPrefs.HasKey("Stage4-all-enemies") && PlayerPrefs.GetInt("Stage4-all-enemies") == 1 &&
                PlayerPrefs.HasKey("Stage5-all-enemies") && PlayerPrefs.GetInt("Stage5-all-enemies") == 1 &&
                PlayerPrefs.HasKey("Stage6-all-enemies") && PlayerPrefs.GetInt("Stage6-all-enemies") == 1)
            {
                DialogManager.StartSequence(specialEndingDialog);
                return;
            }
        }
        if (playOnInit != null)
        {
            DialogManager.StartSequence(playOnInit);
        }   
    }

    private void OnDestroy()
    {
        if (playOnDestroy != null)
        {
            DialogManager.StartSequence(playOnDestroy);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
