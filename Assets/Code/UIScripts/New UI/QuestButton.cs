using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestButton : MonoBehaviour
{
    public enum QuestNames { AppleADay, PostHaste, Lightbulb, Poking };

    [SerializeField]
    public QuestNames questName;
}
