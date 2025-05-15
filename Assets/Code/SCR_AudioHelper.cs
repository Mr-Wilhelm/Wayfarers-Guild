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

    [Header("Book audio")]
    [SerializeField] AudioSource bookAudioSource;
    [SerializeField] AudioClip bookOpen;
    [SerializeField] AudioClip bookClose;

    [Header("Engine audio")]
    [SerializeField] AudioSource engineAudioSource;
    [SerializeField] AudioClip engineNormal;
    [SerializeField] AudioClip engineBoosted;

    [Header("Fuel storage")]
    [SerializeField] AudioSource FuelStorageAudioSource;
    [SerializeField] AudioClip pickUpFuel;

    [Header("Fuse storage")]
    [SerializeField] AudioSource FuseStorageAudioSource;
    [SerializeField] AudioClip pickUpFuse;

    [Header("Bolt storage")]
    [SerializeField] AudioSource BoltStorageAudioSource;
    [SerializeField] AudioClip pickUpBolt;

    [Header("Wheel")]
    [SerializeField] AudioSource wheelAudioSource;
    [SerializeField] AudioClip wheelTurn;

    public static SCR_AudioHelper instance;
    public static SCR_AudioHelper Instance
    {
        get { return instance; }    //intellisense is a literal god, it did all of this automatically
    }

    private void Awake()
    {
        //standard code for preventing duplicate instances of a singleton
        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void CheckAudioSourceLocated(string clipType)
    {
        if (clipType == "ballistaReload" || clipType == "ballistaFire" && ballistaAudioSource == null)
        {
            ballistaAudioSource = GameObject.Find("Ballista Turret (1)").GetComponent<AudioSource>();
            Debug.Log("Ballista audio source is: " + ballistaAudioSource.gameObject.name);
        }

        if (clipType == "OpenBook" || clipType == "CloseBook" && bookAudioSource == null)
        {
            bookAudioSource = GameObject.Find("Compendium export with animation (2)").GetComponent<AudioSource>();
            Debug.Log("Book audio source is: " + bookAudioSource.gameObject.name);
        }

        if (clipType == "Engine" || clipType == "EngineBoosted" && engineAudioSource == null)
        {
            engineAudioSource = GameObject.Find("fire or engine entrance").GetComponent<AudioSource>();
            Debug.Log("engine audio source is: " + engineAudioSource.gameObject.name);
        }

        if(clipType == "PickUpFuel" && FuelStorageAudioSource == null)
        {
            FuelStorageAudioSource = GameObject.Find("Food Crate Audio OBJ").GetComponent<AudioSource>();
        }

        if(clipType == "PickUpBolt" && BoltStorageAudioSource == null)
        {
            BoltStorageAudioSource = GameObject.Find("ballista Storage Audio OBJ").GetComponent<AudioSource>();
        }

        if(clipType == "PickUpFuse" && FuseStorageAudioSource == null)
        {
            FuseStorageAudioSource = GameObject.Find("FuseShelf").GetComponent<AudioSource>();
        }

        if(clipType == "WheelTurn" || clipType == "WheelCenter" && wheelAudioSource == null)
        {
            wheelAudioSource = GameObject.Find("ShipWheel").GetComponent<AudioSource>();
        }
    }

    public void PlayAudioClipAcrossNetwork(string clipType)
    {
        int clipIndex = 0;

        CheckAudioSourceLocated(clipType);

        if(clipType == "ballistaReload")
        {
            clipIndex = Random.Range(0, ballistaReloadList.Count);
        }

        PlayAudioClipServerRPC(clipType, clipIndex);
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayAudioClipServerRPC(string clipType, int clipIndex)
    {
        PlayClip(clipType, clipIndex);

        PlayClipClientRPC(clipType, clipIndex);
    }

    [ClientRpc(RequireOwnership = false)]
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
            case "OpenBook":
                bookAudioSource.PlayOneShot(bookOpen);
                Debug.Log("Playing open book");
                break;
            case "CloseBook":
                bookAudioSource.PlayOneShot(bookClose);
                break;
            case "Engine":
                engineAudioSource.PlayOneShot(engineNormal);
                break;
            case "EngineBoosted":
                engineAudioSource.PlayOneShot(engineBoosted);
                break;
            case "PickUpFuel":
                FuelStorageAudioSource.PlayOneShot(pickUpFuel);
                Debug.Log("Playing fuel pickup");
                break;
            case "PickUpBolt":
                BoltStorageAudioSource.PlayOneShot(pickUpBolt);
                Debug.Log("Playing: " + pickUpBolt);
                break;
            case "PickUpFuse":
                FuseStorageAudioSource.PlayOneShot(pickUpFuse);
                Debug.Log("Playing fuse pickup");
                break;
            case "WheelTurn":
                wheelAudioSource.PlayOneShot(wheelTurn);
                break;
            case "WheelCenter":
                wheelAudioSource.PlayOneShot(wheelTurn);
                break;
            default:
                Debug.LogWarning("clip type not found");
                break;
        }
    }
}
