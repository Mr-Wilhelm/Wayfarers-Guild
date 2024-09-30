using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerCountdown : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI CountdownTimer;

    [SerializeField]
    private float timeLeft;

    [SerializeField]
    private float timeForMission;

    private void Start()
    {
        CountdownTimer = gameObject.GetComponent<TextMeshProUGUI>();
        timeLeft = timeForMission;

    }

    private void Update()
    {
        timeLeft -= Time.deltaTime;

        ShowTimeAsTimer();
    }

    void ShowTimeAsTimer()
    {
        float minutes = Mathf.FloorToInt(timeLeft / 60);
        float seconds = Mathf.Floor(timeLeft % 60);

        CountdownTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
