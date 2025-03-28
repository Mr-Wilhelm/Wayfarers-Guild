using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Collections;
using JetBrains.Annotations;

public class QuestInfo : NetworkBehaviour
{
    public enum questTypes {Cargo, Research };
    public enum npcBroker {None, Jenny, Keegan, Chris, Celia, Matthew };

    public enum questHeading {AppleADay, LightbulbMoment };

    public NetworkVariable<FixedString128Bytes> questText;

    public float rewardMoney;

    public Image questStamp;

    public Image jennyImage;
    public Image deliveryImage;
    public Image researchImage;

    public NetworkVariable<questHeading> activeQuest;
    public NetworkVariable<npcBroker> targetNPC;
    public NetworkVariable<questTypes> questType;

    //network stuff testing

    public NetworkVariable<int> networkActiveQuest;     //0 - Apple A Day, 1 - PostHaste, 2 - Lightbulb, 3 - Poking
    public NetworkVariable<int> networkTargetNPC;
    public NetworkVariable<int> networkQuestType;

    private void Start()
    {
        jennyImage = GameObject.Find("JennyImage").GetComponent<Image>();
        deliveryImage = GameObject.Find("DeliveryImage").GetComponent<Image>();
        researchImage = GameObject.Find("ResearchImage").GetComponent<Image>();
        questStamp = GameObject.Find("Stamp").GetComponent<Image>();
    }
}
