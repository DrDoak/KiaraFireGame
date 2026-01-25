using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
public class AudioManager : MonoBehaviour
{
    private FMOD.Studio.EventInstance mMusicEvent;
    private string lastMusicPath;

    private FMOD.Studio.EventInstance mAmbienceEvent;
    private string lastAmbiencePath;

    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;
    private EventInstance ambienceEventInstance;
    private EventInstance musicEventInstance;
    public GameObject[] environmentAudio;
    [SerializeField]
    private AudioSettings settingsPanel;
    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);    //If there is a second instance of this class destroy it and just return
            return;
        }
        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();
        DontDestroyOnLoad(gameObject);

    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 0.5f);
        }
        if (!PlayerPrefs.HasKey("sfxVolume"))
        {
            PlayerPrefs.SetFloat("sfxVolume", 0.5f);
        }
        //Start Ambience and music
        FMOD.Studio.Bus bus = FMODUnity.RuntimeManager.GetBus("bus:/Music");
        bus.setVolume(PlayerPrefs.GetFloat("musicVolume"));
        FMOD.Studio.Bus sfx = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
        sfx.setVolume(PlayerPrefs.GetFloat("sfxVolume"));
        if (settingsPanel != null)
        {
            settingsPanel.InitializeSettings();
        }
    }

    public void SetAmbienceParameter(string parameterName, float parameterValue)
    {
        ambienceEventInstance.setParameterByName(parameterName, parameterValue);
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return emitter;
    }

    public void EnableEnvironmentAudio()
    {
        foreach (GameObject musicObj in environmentAudio)
        {
            musicObj.SetActive(true);
        }
    }

    public void DisableEnvironmentAudio()
    {
        foreach (GameObject musicObj in environmentAudio)
        {
            musicObj.SetActive(false);
        }
    }

    public static void StopAllMusic()
    {
        if (instance.mMusicEvent.isValid())
        {
            instance.mMusicEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            instance.mMusicEvent.release();
            instance.lastMusicPath = "";
        }
    }
    public static void StopAllAmbience()
    {
        if (instance.mAmbienceEvent.isValid())
        {
            instance.mAmbienceEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            instance.mAmbienceEvent.release();
            instance.lastAmbiencePath = "";
        }
    }

    public static void PlayMusic(EventReference startMusicEvent)
    {
        if (startMusicEvent.IsNull) return;
        if (instance.lastMusicPath != "" && instance.lastMusicPath != GUIDToPath(startMusicEvent.Guid))
        {
            StopAllMusic();
        }
        if (instance.lastMusicPath != GUIDToPath(startMusicEvent.Guid))
        {
            instance.mMusicEvent = FMODUnity.RuntimeManager.CreateInstance(startMusicEvent);
            instance.mMusicEvent.start();
            instance.lastMusicPath = GUIDToPath(startMusicEvent.Guid);
        }
    }

    public static void PlayAmbience(EventReference startMusicEvent)
    {
        if (startMusicEvent.IsNull) return;
        if (instance.lastAmbiencePath != "" && instance.lastAmbiencePath != GUIDToPath(startMusicEvent.Guid))
        {
            StopAllAmbience();
        }
        if (instance.lastAmbiencePath != GUIDToPath(startMusicEvent.Guid))
        {
            instance.mAmbienceEvent = FMODUnity.RuntimeManager.CreateInstance(startMusicEvent);
            instance.mAmbienceEvent.start();
            instance.lastAmbiencePath = GUIDToPath(startMusicEvent.Guid);
        }
    }

    public static string GUIDToPath(FMOD.GUID guid)
    {
        string path;
        RuntimeManager.StudioSystem.lookupPath(guid, out path);
        return path;
    }

    private void CleanUp()
    {
        if (eventInstances != null)
        {
            foreach (EventInstance eventInstance in eventInstances)
            {
                eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                eventInstance.release();
            }
        }
        if (eventEmitters != null)
        {
            foreach (StudioEventEmitter emitter in eventEmitters)
            {
                emitter.Stop();
            }
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}
