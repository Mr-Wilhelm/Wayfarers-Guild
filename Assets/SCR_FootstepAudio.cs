using Gravitas.Demo;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.Netcode;
using UnityEngine;

public class SCR_FootstepAudio : NetworkBehaviour
{
    [SerializeField] AudioSource playerAudioSource;

    [SerializeField] AudioClip metalWalk1;
    [SerializeField] AudioClip metalWalk2;
    [SerializeField] AudioClip metalWalk3;
    [SerializeField] AudioClip metalWalk4;

    [SerializeField] AudioClip woodWalk1;
    [SerializeField] AudioClip woodWalk2;
    [SerializeField] AudioClip woodWalk3;
    [SerializeField] AudioClip woodWalk4;

    public GravitasFirstPersonPlayerSubject gravitasPlayerControllerRef;
    public SCR_ShipControls shipControlsRef;
    private bool footstepPlaying;

    private void Update()
    {
        if (!gravitasPlayerControllerRef.Walking || gravitasPlayerControllerRef.Jumping && footstepPlaying)
        {
            playerAudioSource.Stop();
        }
        if (gravitasPlayerControllerRef.Walking && !shipControlsRef.onWheel && !gravitasPlayerControllerRef.Jumping)
        {
            SelectFootstep("Wood");
        }
    }

    public void SelectFootstep(string typeOfMaterial)
    {
        float footstepToUse = Random.Range(1, 5);
        if(typeOfMaterial == "Wood")
        {
            switch (footstepToUse)
            {
                case 1:
                    PlayFootStepAudio(woodWalk1);
                    break;
                case 2:
                    PlayFootStepAudio(woodWalk2);
                    break;
                case 3:
                    PlayFootStepAudio(woodWalk3);
                    break;
                case 4:
                    PlayFootStepAudio(woodWalk4);
                    break;
                default:
                    Debug.Log("Footstep switch statement glitched");
                    break;
            }
        }
        else
        {
            switch (footstepToUse)
            {
                case 1:
                    PlayFootStepAudio(metalWalk1);
                    break;
                case 2:
                    PlayFootStepAudio(metalWalk2);
                    break;
                case 3:
                    PlayFootStepAudio(metalWalk3);
                    break;
                case 4:
                    PlayFootStepAudio(metalWalk4);
                    break;
                default:
                    Debug.Log("Footstep switch statement glitched");
                    break;
            }
        }
    }

    private void PlayFootStepAudio(AudioClip footstepToPlay)
    {
        if(!footstepPlaying)
        {
            playerAudioSource.PlayOneShot(footstepToPlay);
            footstepPlaying = true;
            Invoke(nameof(SetFootStepPlayingToFalse), footstepToPlay.length);
        }
        else
        {
            Debug.Log("footstep already playing");
        }
        
    }

    private void SetFootStepPlayingToFalse()
    {
        footstepPlaying = false;
    }
}
