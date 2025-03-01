using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestInfo : MonoBehaviour
{
    public enum questTypes {Cargo, Research };
    public enum npcBroker {None, Jenny, Keegam, Chris, Celia, Matthew };

    public enum questHeading {AppleADay, LightbulbMoment };

    public string questText;

    public float rewardMoney;

    public Image jennyImage;

    public questHeading activeQuest;
    public npcBroker targetNPC;
    public questTypes questType;

    private void Start()
    {
        jennyImage = GameObject.Find("JennyImage").GetComponent<Image>();
    }
}
