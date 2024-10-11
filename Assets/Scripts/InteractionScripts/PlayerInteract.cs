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
    private float timeToLerp;

    [SerializeField]
    private float barMoveCountdown;

    [SerializeField]
    private Image lerpingBarImage;

    private void Start()
    {
        barMoveCountdown = 5.0f;
        lerpBarSpeed = 1.0f;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) && canInteract && canDoSpeedMinigame && !isInteracting)
        {
            DoSpeedMinigame();
        }

        else if (Input.GetKeyDown(KeyCode.F) && isInteracting)
        {
            isInteracting = false;
        }
    }

    private void DoSpeedMinigame()
    {
        isInteracting = true;
        barMoveCountdown -= Time.deltaTime;

        while (isInteracting && barMoveCountdown <= 0)
        {
            lerpingBarValToReach = Random.Range(0.0f, 1.0f);

            lerpingBarCurrentVal = Mathf.SmoothDamp(lerpingBarCurrentVal, lerpingBarValToReach, ref lerpBarSpeed, timeToLerp);
            barMoveCountdown = 5.0f;
        }
    }
}
