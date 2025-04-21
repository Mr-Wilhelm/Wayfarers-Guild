using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class SCR_Portal : MonoBehaviour
{
    [SerializeField]
    private QuestHandler questHandler;

    private void Start()
    {
        questHandler = GameObject.Find("PlayerQuestHandler").GetComponent<QuestHandler>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Ship")
        {

            GameObject.Find("PRE-Airship").GetComponent<NetworkObject>().Despawn();
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                player.GetComponent<SCR_ShipControls>().enabled = false;
            }

            if(questHandler.hasCargoQuest.Value == true)    //completion check for cargo quests
            {
                Debug.Log("Cargo Quest Completed");
                Debug.Log("Set quest for completion in the dialogue for spoons");
            }

            GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SCR_SceneManagerScript>().LoadCityScene();
        }
    }
}
