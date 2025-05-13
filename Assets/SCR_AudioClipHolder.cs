using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SCR_AudioClipHolder : NetworkBehaviour
{
    // Start is called before the first frame update
    [Serializable]
    public struct NamedAudioClip
    {
        public string name;
        public AudioClip clip;
    }

    [Header("Named Audio Clips")]
    public List<NamedAudioClip> audioClipList;

    private Dictionary<string, AudioClip> soundMap;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        soundMap = new Dictionary<string, AudioClip>(StringComparer.OrdinalIgnoreCase);
        foreach(var audioClip in audioClipList)
        {
            if(!string.IsNullOrEmpty(audioClip.name) && audioClip.name != null)
            {
                soundMap[audioClip.name] = audioClip.clip;
            }
        }
    }

    public AudioClip GetClip(string name)
    {
        if(soundMap.TryGetValue(name, out var clip)) { return clip; }
        Debug.LogWarning($"[AudioClipHolder] No clip found for name '{name}' on {gameObject.name}");
        return null;
    }

    public AudioClip GetClip(Enum soundEnum)
    {
        return GetClip(soundEnum.ToString());
    }
}
