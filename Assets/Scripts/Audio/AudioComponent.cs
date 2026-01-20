using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioComponent : MonoBehaviour
{
    // Start is called before the first frame update\
    [SerializeField]
    protected AudioEventList audioEventList;
    [SerializeField]
    protected List<StudioEventEmitter> events = new List<StudioEventEmitter>();

    protected Dictionary<string, StudioEventEmitter> eventDict = new Dictionary<string, StudioEventEmitter>();
    private bool initialized = false;
    void Start()
    {
        audioEventList.MapSounds();
        if (FMODEvents.instance != null)
        {
            FMODEvents.instance.LoadSounds(audioEventList.allSounds);
            foreach (StudioEventEmitter emitter in events)
            {
                eventDict[emitter.gameObject.name] = emitter;
            }
            initialized = true;
        }
    }
    private void Update()
    {
        if (!initialized)
        {
            if (FMODEvents.instance != null)
            {
                FMODEvents.instance.LoadSounds(audioEventList.allSounds);
                foreach (StudioEventEmitter emitter in events)
                {
                    eventDict[emitter.gameObject.name] = emitter;
                }
                initialized = true;
            }
        }
    }

    public void PlaySounds(string sound)
    {
        AudioManager.instance.PlayOneShot(audioEventList.GetSoundReference(sound), Vector3.zero);
    }

    public void StartEvent(string sound)
    {
        if (!eventDict.ContainsKey(sound)) return;
        eventDict[sound].Play();
    }
    public void StopAllEvents()
    {
        foreach (StudioEventEmitter emitter in events)
        {
            emitter.Stop();
        }
    }

    public void StopEvent(string sound)
    {
        if (!eventDict.ContainsKey(sound)) return;
        eventDict[sound].Stop();
    }


    public void PlayCurrentFootstepSound()
    {
        
    }

}
