using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;
using static SCR_FootstepAudio;
using UnityEngine.Audio;
using GLTFast.Schema;

public class SCR_AudioHelper : NetworkBehaviour
{
    [Header("Ballista audio")]
    [SerializeField] AudioSource ballistaAudioSource;
    [SerializeField] AudioClip ballistaFire;
    [SerializeField] List<AudioClip> ballistaReloadList = new List<AudioClip>();

    public void PlayAudioClipAcrossNetwork(string clipType)
    {
        int clipIndex = 0;

        if(clipType == "ballistaReload")
        {
            clipIndex = Random.Range(0, ballistaReloadList.Count);
        }

        PlayAudioClipServerRPC(clipType, clipIndex);
    }

    [ServerRpc]
    private void PlayAudioClipServerRPC(string clipType, int clipIndex)
    {
        PlayClip(clipType, clipIndex);

        PlayClipClientRPC(clipType, clipIndex);
    }

    [ClientRpc]
    private void PlayClipClientRPC(string clipType, int clipIndex)
    {
        PlayClip(clipType, clipIndex);
    }

    private void PlayClip(string clipType, int clipIndex)
    {
        switch(clipType)
        {
            case "ballistaReload":
                ballistaAudioSource.PlayOneShot(ballistaReloadList[clipIndex]);
                break;
            case "ballistaFire":
                ballistaAudioSource.PlayOneShot(ballistaFire);
                break;
            default:
                Debug.LogWarning("clip type not found");
                break;
        }
    }
}
