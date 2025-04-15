using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

public class QuestHandler : NetworkBehaviour
{
    private static QuestHandler instance;

    public NetworkVariable<FixedString128Bytes> activeQuest = new NetworkVariable<FixedString128Bytes>();

    public NetworkVariable<FixedString128Bytes> activeQuestDescription = new NetworkVariable<FixedString128Bytes>();

    [SerializeField]
    private string appleADayQuestTask;

    [SerializeField]
    private string postHasteQuestTask;

    [SerializeField]
    private string lightbulbQuestTask;

    [SerializeField]
    private string pokingQuestTask;

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

    private void Start()
    {
        appleADayQuestTask = "Complete a run with the cargo still in tact";
        postHasteQuestTask = "Complete a run with the cargo still in tact";
        lightbulbQuestTask = "Attract a Voracious Angel Moth by turning on the ship lights";
        pokingQuestTask = "Attract a Tulebreather with the sonar ping";

        //activeQuest.OnValueChanged += QuestChanged; //subscribes OnValueChanged with a delegate by using +=. This tells the code to call the function when OnValueChanged happens
    }

    private void Update()
    {
        Debug.Log("Current Active Quest: " + activeQuest.Value);
        Debug.Log(" Current Quest Description " + activeQuestDescription.Value);
    }

    private void QuestChanged(FixedString128Bytes oldQuest, FixedString128Bytes newQuest)
    {
        AssignQuestTask();
    }

    private void AssignQuestTask()
    {
        string questName = activeQuest.Value.ToString();

        switch (questName)
        {
            case "AppleADay":
                activeQuestDescription.Value = appleADayQuestTask;
                break;
            case "Poking":
                activeQuestDescription.Value = pokingQuestTask;
                break;
            case "LightbulbMoment":
                activeQuestDescription.Value = lightbulbQuestTask;
                break;
            case "PostHaste":
                activeQuestDescription.Value = postHasteQuestTask;
                break;
        }
    }
}
