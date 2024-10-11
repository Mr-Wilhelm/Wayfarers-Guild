using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    public bool canInteract;

    public bool canDoSpeedMinigame;

    public bool isInteracting;

    [Header("AutoLerping Bar Settings")]
    [SerializeField]
    private float lerpingBarCurrentVal;

    [SerializeField]
    private float lerpingBarValToReach;

    [SerializeField]
    private float pointInLerp;

    [SerializeField]
    private float lerpRate;

    [SerializeField]
    private float barMoveCountdown;

    [SerializeField]
    private Image lerpingBarImage;

    [Header("Player Bar Settings")]

    [SerializeField]
    private float playerBarVal;

    [SerializeField]
    private float playerBarMoveRate;

    [SerializeField]
    private Image playerBarImage;

    [Header("Bar Range Checks")]
    [SerializeField]
    private float barSuccessRange;

    private void Start()
    {
        barMoveCountdown = 2.0f;
        lerpRate = 2f;

        playerBarVal = 0.5f;
        playerBarMoveRate = 0.5f;

        barSuccessRange = 0.1f;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) && canInteract && canDoSpeedMinigame && !isInteracting)
        {
            isInteracting = true;
        }

        else if (Input.GetKeyDown(KeyCode.F) && isInteracting)
        {
            isInteracting = false;
        }

        if(isInteracting && canDoSpeedMinigame)
        {
            DoSpeedMinigame();
            MovePlayerBar();
            CheckBarValues();
        }
    }

    private void DoSpeedMinigame()
    {
        MoveSpeedBar();
    }

    /// <summary>
    /// Lerps the bar to a random value between 0 and 1
    /// bar is clamped so that it does not go under 0 or over 1
    /// 
    /// </summary>
    private void MoveSpeedBar()
    {
        barMoveCountdown -= Time.deltaTime;

        //reset the timer
        if (barMoveCountdown <= 0)
        {
            lerpingBarValToReach = Random.Range(0.0f, 1.0f);
            barMoveCountdown = Random.Range(2.0f, 5.0f);
        }

        //this code is probably overcomplicated, but it works so im not touching it anymore lol

        //lerp down to the value
        if (lerpingBarCurrentVal > lerpingBarValToReach)
        {
            pointInLerp = lerpRate* Time.deltaTime;

            //clamp
            if (pointInLerp <= 0.0f)
            {
                pointInLerp = 0.0f;
            }
        }

        //lerp up to the value
        else if (lerpingBarCurrentVal < lerpingBarValToReach)
        {
            pointInLerp = lerpRate* Time.deltaTime;

            //clamp
            if (pointInLerp >= 1.0f)
            {
                pointInLerp = 1.0f;
            }
        }

        //set the bar value to lerp, and move the fill amount
        lerpingBarCurrentVal = Mathf.Lerp(lerpingBarCurrentVal, lerpingBarValToReach, pointInLerp);
        lerpingBarImage.fillAmount = lerpingBarCurrentVal;
    }

    private void MovePlayerBar()
    {
        if (Input.GetKey(KeyCode.D) && playerBarVal <= 1.0f)
        {
            playerBarVal += playerBarMoveRate * Time.deltaTime;
        }

        if(Input.GetKey(KeyCode.A) && playerBarVal >= 0.0f)
        {
            playerBarVal -= playerBarMoveRate * Time.deltaTime;
        }

        playerBarImage.fillAmount = playerBarVal;
    }

    private void CheckBarValues()
    {
        //if it is greater than -0.1 and less than 0.1 (or whatever the barSuccessRange is
        if(playerBarVal - lerpingBarCurrentVal >= -barSuccessRange && playerBarVal - lerpingBarValToReach <= barSuccessRange)
        {
            Debug.Log("Winning");
        }
    }
}
