using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System.Text.RegularExpressions;

//Meant to store all sound effects
public class FMODEvents : MonoBehaviour
{
    public static FMODEvents instance {  get; private set; }
    private Dictionary<string, EventReference> currentSounds = new Dictionary<string, EventReference>();

    private void Awake()
    {
        instance = this;
    }

    public void LoadSounds(List<EventReference> sounds)
    {
        foreach (EventReference sound in sounds)
        {
            if(currentSounds.ContainsValue(sound))
            {
                continue;
            }
            string key = Regex.Match(sound.ToString(), @"/\s*(.+?)\s*\)").Groups[1].Value;
            currentSounds[key] = sound;
        }
    }

    public EventReference GetSound(string name)
    {
        if(currentSounds.ContainsKey(name))
        {
            return currentSounds[name];
        }
        return default(EventReference);
    }
}
