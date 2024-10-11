using System.Collections;
using System.Collections.Generic;
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
    private float lerpBarSpeed;

    [SerializeField]
    private float pointInLerp;

    [SerializeField]
    private float barMoveCountdown;

    [SerializeField]
    private Image lerpingBarImage;

    private void Start()
    {
        barMoveCountdown = 2.0f;
        lerpBarSpeed = 1.0f;
        
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
        pointInLerp += 0.5f * Time.deltaTime;

        //note to self, variable t in lerp is the point it is at between the two values, not the time it takes to get from one to the other

        //if(pointInLerp >= 1.0f)
        //{
        //    pointInLerp = 0.0f;
        //}

        //if (barMoveCountdown <= 0)
        //{
        //    lerpingBarValToReach = Random.Range(0.0f, 1.0f);

        //    lerpingBarCurrentVal = Mathf.Lerp(lerpingBarCurrentVal, lerpingBarValToReach, pointInLerp);

        //    lerpingBarImage.fillAmount = lerpingBarCurrentVal;
        //    barMoveCountdown = Random.Range(2.0f, 5.0f);
        //}
    }
}
