using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class GameUIScript : NetworkBehaviour
{
    public QuestHandler questHandlerObject;

    public NetworkVariable<FixedString128Bytes> questPrompt = new NetworkVariable<FixedString128Bytes>();
    public NetworkVariable<FixedString128Bytes> questPromptHeading = new NetworkVariable<FixedString128Bytes>();
    public NetworkVariable<FixedString128Bytes> questPromptTask = new NetworkVariable<FixedString128Bytes>();

    [SerializeField]
    private TextMeshProUGUI questPromptText, questPromptHeadingText, questPromptTaskText;

    [SerializeField]
    private bool questPromptEnabled = false;

    //Start is called before the first frame update
    void Start()
    {
        questHandlerObject = GameObject.Find("PlayerQuestHandler").GetComponent<QuestHandler>();

        //getting the variables
        questPromptText = GameObject.Find("QuestPrompt").GetComponent<TextMeshProUGUI>();
        questPromptHeadingText = GameObject.Find("QuestPromptHeading").GetComponent<TextMeshProUGUI>();
        questPromptTaskText = GameObject.Find("QuestPromptTask").GetComponent<TextMeshProUGUI>();

        //getting the values from the Quest Handler for the active quest
        questPrompt.Value = "Hold Q to view current quest";
        questPromptHeading.Value = questHandlerObject.activeQuest.Value;
        questPromptTask.Value = questHandlerObject.activeQuestDescription.Value;

        //assinging the text to the value of the network string
        questPromptText.text = questPrompt.Value.ToString();
        questPromptHeadingText.text = questPromptHeading.Value.ToString();
        questPromptTaskText.text = questPromptTask.Value.ToString();

        questPromptText.gameObject.SetActive(true);
        questPromptHeadingText.gameObject.SetActive(false);
        questPromptTaskText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            questPromptEnabled = !questPromptEnabled;
        }
        if(Input.GetKeyUp(KeyCode.Q))
        {
            questPromptEnabled = !questPromptEnabled;
        }

        if(questPromptEnabled)
        {
            Debug.Log("Show Quest");
            questPromptText.gameObject.SetActive(false);
            questPromptHeadingText.gameObject.SetActive(true);
            questPromptTaskText.gameObject.SetActive(true);
        }
        else if(!questPromptEnabled)
        {
            //Debug.Log("Hide Quest");
            questPromptText.gameObject.SetActive(true);
            questPromptHeadingText.gameObject.SetActive(false);
            questPromptTaskText.gameObject.SetActive(false);
        }
    }
}
