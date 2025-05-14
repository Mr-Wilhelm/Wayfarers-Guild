using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestInfo : MonoBehaviour
{
    public enum questTypes { Cargo, Research };
    public enum npcBroker { None, Jenny, Keegan, Chris, Celia, Matthew };

    public enum questHeading { AppleADay, LightbulbMoment, Poking, PostHaste };

    public string questText;

    public float rewardMoney;

    public Image questStamp;

    public Image jennyImage;
    public Image deliveryImage;
    public Image researchImage;

    public questHeading activeQuest;
    public npcBroker targetNPC;
    public questTypes questType;

    private void Start()
    {
        questStamp = GameObject.Find("Stamp").GetComponent<Image>(); Debug.Log("STAMP FOUND");
        jennyImage = GameObject.Find("JennyImage").GetComponent<Image>(); Debug.Log("JENNY FOUND");
        deliveryImage = GameObject.Find("DeliveryImage").GetComponent<Image>(); Debug.Log("DELIVERY FOUND");
        researchImage = GameObject.Find("ResearchImage").GetComponent<Image>(); Debug.Log("RESEARCH FOUND");

    }
}
