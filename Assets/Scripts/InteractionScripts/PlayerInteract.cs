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

    public bool canSteerShip;

    [SerializeField]
    private GameObject speedMiniGameObj;

    [SerializeField]
    private ControlWorld shipLogic;

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
    private float barMoveCountdownMin;

    [SerializeField]
    private float barMoveCountdownMax;

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
        barMoveCountdown = barMoveCountdownMin;

        barMoveCountdownMin = 0.5f;
        barMoveCountdownMax = 2.0f;
        lerpRate = 2f;

        playerBarVal = 0.5f;
        playerBarMoveRate = 0.5f;

        barSuccessRange = 0.1f;

        speedMiniGameObj = GameObject.Find("PressureGauge");
        shipLogic = GameObject.Find("WorldCen").GetComponent<ControlWorld>();

        //get the canvas of the pressure gauge
        //then get either the lerping bar (index 1) or the player bar (index 3) from that canvas
        lerpingBarImage = speedMiniGameObj.transform.GetChild(3).transform.GetChild(1).GetComponent<Image>();   //(I hate that this is how you do this lol)
        playerBarImage = speedMiniGameObj.transform.GetChild(3).transform.GetChild(3).GetComponent<Image>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) && canInteract && !isInteracting)
        {
            isInteracting = true;
        }

        else if (Input.GetKeyDown(KeyCode.F) && isInteracting)
        {
            isInteracting = false;
            shipLogic.playerControllingShip = false;
        }

        if(isInteracting && canDoSpeedMinigame)
        {
            DoSpeedMinigame();
            MovePlayerBar();
            CheckBarValues();
        }

        else if(isInteracting && canSteerShip)
        {
            shipLogic.playerControllingShip = true; // the amazing wonderfull contributions of Edward 'Danger' Hayden
            Debug.Log("Joe smellz");
            shipLogic.AirshipYaw = Input.GetAxisRaw("Horizontal");
            shipLogic.AirshipThrust = Input.GetAxisRaw("Vertical");

            shipLogic.AirshipRoll = Input.GetAxisRaw("AirshipRoll");
            shipLogic.AirshipPitch = Input.GetAxisRaw("AirshipPitch");

            shipLogic.AirshipAscend = Input.GetAxisRaw("AirshipAscend");
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
            barMoveCountdown = Random.Range(barMoveCountdownMin, barMoveCountdownMax);
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

    /// <summary>
    /// Moves the player bar up and down
    /// </summary>
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
            shipLogic.moveSpeed = 10;
            shipLogic.rotSpeed = 60;
        }
        else
        {
            shipLogic.moveSpeed = 5;
            shipLogic.rotSpeed = 40;
        }
    }
}
