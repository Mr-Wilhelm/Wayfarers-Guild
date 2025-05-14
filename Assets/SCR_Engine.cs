using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SCR_Engine : NetworkBehaviour
{
    private bool boostedAudioPlaying = false;

    public AudioSource engineAudioSource;
    [SerializeField] SCR_AudioHelper audioHelper;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        audioHelper = GameObject.Find("AudioHelperOBJ").GetComponent<SCR_AudioHelper>();
        PlayConstantEngineSound();
    }

    private void PlayConstantEngineSound()
    {
        Debug.Log("Restarting engine noise");
        audioHelper.PlayAudioClipAcrossNetwork("Engine");
        CancelInvoke(nameof(RestartBasicEngineSound));
        Invoke(nameof(RestartBasicEngineSound), 30.0f);
    }

    public void PlayBoostedEngineSound()
    {
        Debug.Log("Attempting to play boosted engine sound");
        engineAudioSource.Stop();
        audioHelper.PlayAudioClipAcrossNetwork("EngineBoosted");
        CancelInvoke(nameof(RestartBasicEngineSound));
        Invoke(nameof(RestartBasicEngineSound), 7.0f);
    }

    private void RestartBasicEngineSound()
    {
        engineAudioSource.Stop();
        PlayConstantEngineSound();
    }
}
