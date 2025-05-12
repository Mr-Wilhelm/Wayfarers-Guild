using Gravitas.Demo;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.Netcode;
using UnityEngine;

public class SCR_FootstepAudio : NetworkBehaviour
{
    public enum FootstepType { Wood1, Wood2, Wood3, Wood4 }

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip wood1;
    [SerializeField] private AudioClip wood2;
    [SerializeField] private AudioClip wood3;
    [SerializeField] private AudioClip wood4;

    public GravitasFirstPersonPlayerSubject gravitasPlayerControllerRef;
    public SCR_ShipControls shipControlsRef;
    private bool footstepPlaying;

    private float footstepCooldown = 0.5f;
    private float lastFootstepTime;

    private void Update()
    {
        if (!IsOwner) return;

        // Replace this with your real walking condition
        bool isWalking = true;

        if (!gravitasPlayerControllerRef.Walking || gravitasPlayerControllerRef.Jumping && footstepPlaying)
        {
            audioSource.Stop();
        }
        if (gravitasPlayerControllerRef.Walking && !shipControlsRef.onWheel && !gravitasPlayerControllerRef.Jumping)
        {
            isWalking = true;
        }
        else
        {
            isWalking = false;
        }

        if (isWalking && Time.time > lastFootstepTime + footstepCooldown)
        {
            lastFootstepTime = Time.time;
            var type = GetRandomFootstep();
            PlayFootstepServerRpc(type);
        }
    }

    private FootstepType GetRandomFootstep()
    {
        return (FootstepType)Random.Range(0, 4);
    }

    [ServerRpc]
    private void PlayFootstepServerRpc(FootstepType footstep)
    {
        // Play on host
        PlayFootstep(footstep);

        // Tell all other clients to play at this object's position
        PlayFootstepClientRpc(footstep);
    }

    [ClientRpc]
    private void PlayFootstepClientRpc(FootstepType footstep)
    {
        //if (IsOwner) return; // Don't double-play

        PlayFootstep(footstep);
    }

    private void PlayFootstep(FootstepType type)
    {
        if (!audioSource) return;

        AudioClip clip = type switch
        {
            FootstepType.Wood1 => wood1,
            FootstepType.Wood2 => wood2,
            FootstepType.Wood3 => wood3,
            FootstepType.Wood4 => wood4,
            _ => null
        };

        if (clip != null)
            audioSource.PlayOneShot(clip);
    }
}
