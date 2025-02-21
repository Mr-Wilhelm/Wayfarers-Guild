using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestButton : MonoBehaviour
{
    [SerializeField]
    private QuestInfo infoToDisplay;

    private void Start()
    {
        infoToDisplay = GameObject.Find("QuestInfo").GetComponent<QuestInfo>();
    }
}
