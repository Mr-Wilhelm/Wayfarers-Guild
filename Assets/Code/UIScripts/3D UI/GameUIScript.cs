using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class GameUIScript : NetworkBehaviour
{
    public QuestHandler questHandlerObject;

    //public NetworkVariable<TextMeshProUGUI> questPrompt = new NetworkVariable<TextMeshProUGUI>();
    //public NetworkVariable<TextMeshProUGUI> questPromptHeading = new NetworkVariable<TextMeshProUGUI>();
    //public NetworkVariable<TextMeshProUGUI> questPromptTask = new NetworkVariable<TextMeshProUGUI>();

    //Start is called before the first frame update
    void Start()
    {
        questHandlerObject = GameObject.Find("PlayerQuestHandler").GetComponent<QuestHandler>();

        //questPrompt.Value.gameObject.SetActive(true);
        //questPromptHeading.Value.gameObject.SetActive(false);
        //questPromptTask.Value.gameObject.SetActive(false);

        //questPromptHeading.Value.text = questHandlerObject.activeQuest.Value.ToString();
        //questPromptTask.Value.text = questHandlerObject.activeQuestDescription.Value.ToString();
    }
}
