using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TimerCountdown : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI CountdownTimer;

    //the current amount of time left for the mission
    [SerializeField]
    private float timeLeft;

    //the total time for the mission in seconds
    [SerializeField]
    private float timeForMission;

    [SerializeField]
    private SceneManagerScript sceneManager;

    private void Start()
    {
        CountdownTimer = gameObject.GetComponent<TextMeshProUGUI>(); 
    }

    private void Update()
    {
        timeLeft += Time.deltaTime;

        ShowTimeAsTimer();
    }

    void ShowTimeAsTimer()
    {
        //dividing the total time by 60 gives the minutes
        float minutes = Mathf.FloorToInt(timeLeft / 60);

        //using the modulo operation on the time gives seconds
        //modulo finds the remainder after dividing the number (so the remainder of timeLeft / 60
        float seconds = Mathf.Floor(timeLeft % 60);

        //string.Format allows to place variables inside of a formatted string
        //{0:00} - the first zero means variable 0 (minutes), the two double zeros are a double digit format placeholder
        //{1:00} - the first one means variable 1 (seconds), the two zeros are a double digit format placeholder
        // minutes is set to be variable 0, seconds is set to be variable 1.
        CountdownTimer.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if(timeLeft >= timeForMission)
        {
            sceneManager.LoadWinScene();
        }
    }
}
