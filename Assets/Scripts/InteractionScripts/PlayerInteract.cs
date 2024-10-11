using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    public bool canInteract;

    public bool canDoSpeedMinigame;

    public bool isInteracting;

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

    private void Start()
    {
        barMoveCountdown = 2.0f;
        lerpRate = 0.01f;
        
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

        if(isInteracting)
        {
            DoSpeedMinigame();
        }
    }

    private void DoSpeedMinigame()
    {
        barMoveCountdown -= Time.deltaTime;

        if(barMoveCountdown <= 0)
        {
            lerpingBarValToReach = Random.Range(0.0f, 1.0f);
            barMoveCountdown = Random.Range(2.0f, 5.0f);
        }

        if (lerpingBarCurrentVal > lerpingBarValToReach)
        {
            pointInLerp -= lerpRate * Time.deltaTime;
        }

        else if (lerpingBarCurrentVal < lerpingBarValToReach)
        {
            pointInLerp += lerpRate * Time.deltaTime;
        }

        lerpingBarCurrentVal = Mathf.Lerp(lerpingBarCurrentVal, lerpingBarValToReach, pointInLerp);
        lerpingBarImage.fillAmount = lerpingBarCurrentVal;
    }
}
