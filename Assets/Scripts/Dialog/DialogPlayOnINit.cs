using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogPlayOnINit : MonoBehaviour
{
    public DialogSequence playOnInit;
    public DialogSequence playOnDestroy;

    // Start is called before the first frame update
    void Start()
    {
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
