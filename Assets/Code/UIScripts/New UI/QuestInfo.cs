using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class QuestInfo : NetworkBehaviour
{
    public enum questTypes {Cargo, Research };
    public enum npcBroker {None, Jenny, Keegan, Chris, Celia, Matthew };

    public enum questHeading {AppleADay, LightbulbMoment };

    public NetworkVariable<string> questText;

    public float rewardMoney;

    public Image questStamp;

    public Image jennyImage;
    public Image deliveryImage;
    public Image researchImage;

    public NetworkVariable<questHeading> activeQuest;
    public NetworkVariable<npcBroker> targetNPC;
    public NetworkVariable<questTypes> questType;

    private void Start()
    {
        jennyImage = GameObject.Find("JennyImage").GetComponent<Image>();
        deliveryImage = GameObject.Find("DeliveryImage").GetComponent<Image>();
        researchImage = GameObject.Find("ResearchImage").GetComponent<Image>();
        questStamp = GameObject.Find("Stamp").GetComponent<Image>();
    }
}
