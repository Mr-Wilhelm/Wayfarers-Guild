using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class QuestHandler : NetworkBehaviour
{
    public static QuestHandler instance;
    
    //tracking variables
    public NetworkVariable<FixedString128Bytes> activeQuest = new NetworkVariable<FixedString128Bytes>();
    public NetworkVariable<FixedString128Bytes> activeQuestDescription = new NetworkVariable<FixedString128Bytes>();

    //Task Descriptions for each quest
    private FixedString128Bytes appleADayQuestTask = "Complete a run with the cargo still intact";
    private FixedString128Bytes postHasteQuestTask = "Complete a run with the cargo still intact";
    private FixedString128Bytes lightbulbQuestTask = "Attract a Voracious Angel Moth by turning on the ship lights";
    private FixedString128Bytes pokingQuestTask = "Attract a Tulebreather with the sonar ping";

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
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    public override void OnNetworkSpawn()   //happens when network stuff starts
    {
        
        activeQuest.OnValueChanged += OnQuestChanged;   //subscribes OnValueChanged with a delegate by using +=. This tells the code to call the function when OnValueChanged happens
        activeQuestDescription.OnValueChanged += OnQuestDescriptionChanged;
    }

    private void Update()
    {
        //Debug.Log("Current Active Quest: " + activeQuest.Value);
        //Debug.Log("Current Quest Description: " + activeQuestDescription.Value);
    }

    //these functions are called when OnValueChanged happens.
    private void OnQuestChanged(FixedString128Bytes oldQuest, FixedString128Bytes newQuest)
    {
        if (IsServer)
        {
            UpdateQuestDescription(newQuest);
        }

    }

    private void OnQuestDescriptionChanged(FixedString128Bytes oldDesc, FixedString128Bytes newDesc)
    {
        Debug.Log("Updated Quest Description: " + newDesc.ToString());
    }

    //actually changes the quest description
    private void UpdateQuestDescription(FixedString128Bytes questName)
    {
        string quest = questName.ToString();

        switch (quest)
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

    [ServerRpc(RequireOwnership = false)]   //allows client to invoke on the server, allows client to have authority over changing stuff

    //This function was made with heavy aid from ChatGPT.
    public void SetQuestServerRpc(string questName)
    {
        // Update the active quest.
        activeQuest.Value = questName;
        // Also update the description immediately.
        UpdateQuestDescription(activeQuest.Value);
    }
}
