using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AudioSettings : MonoBehaviour
{
    public Slider volumeSlider;
    public Slider sfxSlider;

    private void OnEnable()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("musicVolume", 0.5f);
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume", 0.5f);
    }
    public void musicVolumeChange(float val)
    {
        FMOD.Studio.Bus bus = FMODUnity.RuntimeManager.GetBus("bus:/Music");
        bus.setVolume(val);
        PlayerPrefs.GetFloat("musicVolume", val);
    }
    public void sfxVolumeChange(float val)
    {
        FMOD.Studio.Bus sfx = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
        sfx.setVolume(val);
        PlayerPrefs.GetFloat("sfxVolume", val);
    }
}
