using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SCR_NewUiManager : MonoBehaviour
{

    [Header("Buttons")]
    [SerializeField]
    private Button spoonsButton;

    [Header("Sprites")]
    [SerializeField]
    private GameObject spoonsSprite;

    [Header("Animations")]
    [SerializeField]
    private Animator cityAnimator;

    private void Start()
    {
        spoonsButton = GameObject.Find("BUTTON_Spoons").GetComponent<Button>();

        spoonsSprite = GameObject.Find("SPRITE_SpoonsLady");

        cityAnimator = GetComponent<Animator>();
    }

    public void Func_SpoonsButtonPressed()
    {
        cityAnimator.SetBool("SpoonsPressed", true);
    }
}
