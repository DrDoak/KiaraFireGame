using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
[CreateAssetMenu(fileName = "AudioEventList", menuName = "Holojam/Audio")]
public class AudioEventList : ScriptableObject
{
    [Header("Audio Events")]
    public List<EventReference> allSounds;
    protected Dictionary<string, EventReference> soundMap = new Dictionary<string, EventReference>();
    private bool mapped = false;
    public EventReference GetSoundReference(string key)
    {
        if (!mapped)
        {
            MapSounds();
        }
        if (soundMap.ContainsKey(key.ToLower()))
        {
            return soundMap[key.ToLower()];
        } else
        {
            return soundMap["first"];
        }
        
    }

    public void MapSounds()
    {
        foreach (EventReference er in allSounds)
        {
            string path = getPath(er.Guid);
            string key = path.ToString().Substring(path.ToString().LastIndexOf('/') + 1);
            key = key.Replace(")", "");
            soundMap[key.ToLower()] = er;
        }
        mapped = true;
    }

    static public string getPath(FMOD.GUID guid)
    {
        string path = string.Empty;

        RuntimeManager.StudioSystem.lookupPath(guid, out path);

        return path;
    }
}
