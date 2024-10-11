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
    private float timeToLerp;

    [SerializeField]
    private Image lerpingBarImage;

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

        if (isInteracting)
        {
            lerpingBarValToReach = Random.Range(0.0f, 1.0f);
            timeToLerp = Random.Range(2.0f, 5.0f);

            lerpingBarCurrentVal = Mathf.Lerp(lerpingBarCurrentVal, lerpingBarValToReach, timeToLerp * 10);
            lerpingBarImage.fillAmount = lerpingBarCurrentVal;
        }
    }
}
