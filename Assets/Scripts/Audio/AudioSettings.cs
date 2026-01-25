using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AudioSettings : MonoBehaviour
{
    public Slider volumeSlider;
    public Slider sfxSlider;
    public AudioComponent mAudio;

    private void Start()
    {
        mAudio = GetComponent<AudioComponent>();
    }
    private void OnEnable()
    {
        
        InitializeSettings();
    }
    public void InitializeSettings()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("musicVolume", 0.5f);
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume", 0.5f);
    }
    public void musicVolumeChange(float val)
    {
        FMOD.Studio.Bus bus = FMODUnity.RuntimeManager.GetBus("bus:/Music");
        bus.setVolume(val);
        PlayerPrefs.SetFloat("musicVolume", val);
    }
    public void sfxVolumeChange(float val)
    {
        FMOD.Studio.Bus sfx = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
        sfx.setVolume(val);
        PlayerPrefs.SetFloat("sfxVolume", val);
    }
}
