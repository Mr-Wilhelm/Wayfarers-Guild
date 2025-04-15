using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class GameUIScript : NetworkBehaviour
{
    public QuestHandler questHandlerObject;

    //public NetworkVariable<TextMeshProUGUI> questPrompt = new NetworkVariable<TextMeshProUGUI>();
    //public NetworkVariable<TextMeshProUGUI> questPromptHeading = new NetworkVariable<TextMeshProUGUI>();
    //public NetworkVariable<TextMeshProUGUI> questPromptTask = new NetworkVariable<TextMeshProUGUI>();

    public NetworkVariable<FixedString128Bytes> questPrompt = new NetworkVariable<FixedString128Bytes>();
    public NetworkVariable<FixedString128Bytes> questPromptHeading = new NetworkVariable<FixedString128Bytes>();
    public NetworkVariable<FixedString128Bytes> questPromptTask = new NetworkVariable<FixedString128Bytes>();

    [SerializeField]
    private TextMeshProUGUI questPromptText, questPromptHeadingText, questPromptTaskText;

    //Start is called before the first frame update
    void Start()
    {
        questHandlerObject = GameObject.Find("PlayerQuestHandler").GetComponent<QuestHandler>();

        questPromptText = GameObject.Find("QuestPrompt").GetComponent<TextMeshProUGUI>();
        questPromptHeadingText = GameObject.Find("QuestPromptHeading").GetComponent<TextMeshProUGUI>();
        questPromptTaskText = GameObject.Find("QuestPromptTask").GetComponent<TextMeshProUGUI>();

        //questPromptHeading.Value.text = questHandlerObject.activeQuest.Value.ToString();
        //questPromptTask.Value.text = questHandlerObject.activeQuestDescription.Value.ToString();
    }
}
