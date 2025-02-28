using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestInfo : MonoBehaviour
{
    public enum questTypes {Cargo, Research };
    public enum npcBroker {None, Jenny, Keegam, Chris, Celia, Matthew };

    public enum questHeading {AppleADay, LightbulbMoment };

    public string questText;

    public float rewardMoney;

    public questHeading activeQuest;
    public npcBroker targetNPC;
    public questTypes questType;
}
