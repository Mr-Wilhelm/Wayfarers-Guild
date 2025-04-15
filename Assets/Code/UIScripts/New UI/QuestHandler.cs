using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class QuestHandler : NetworkBehaviour
{
    private static QuestHandler instance;

    public NetworkVariable<FixedString128Bytes> activeQuest = new NetworkVariable<FixedString128Bytes>();

    public static QuestHandler Instance
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
        else if(instance != this)
        {
            Destroy(gameObject);
            return;
        }

    }

    private void Update()
    {
        Debug.Log("Current Active Quest: " + activeQuest.Value);
    }
}
