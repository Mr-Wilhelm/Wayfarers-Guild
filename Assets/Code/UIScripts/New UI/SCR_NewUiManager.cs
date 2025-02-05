using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Ink.Runtime;
using TMPro;

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

    [Header("Dialogue System Variables")]
    [SerializeField]
    private TextAsset spoonsNPCDialogue;

    [SerializeField]
    private GameObject dialoguePanel;

    [SerializeField]
    private TextMeshProUGUI dialogueText;

    [SerializeField]
    private Story currentStory;

    [SerializeField]
    private bool dialogueIsPlaying = false;

    [SerializeField]
    private GameObject[] choices;

    [SerializeField]
    private TextMeshProUGUI[] choicesText;

    private Coroutine typeTextCoroutine;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip textSound;

    private void Start()
    {
        spoonsButton = GameObject.Find("BUTTON_Spoons").GetComponent<Button>();

        spoonsSprite = GameObject.Find("SPRITE_SpoonsLady");

        cityAnimator = GetComponent<Animator>();

        //dialogue system variable assignment
        spoonsNPCDialogue = Resources.Load<TextAsset>("InkJsons/NPC1");

        dialoguePanel = GameObject.Find("DialogueBox");
        dialoguePanel.SetActive(false);
        dialogueIsPlaying = false;
        dialogueText = dialoguePanel.GetComponentInChildren<TextMeshProUGUI>();
        //dialogue system variable assignment

        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(!dialogueIsPlaying)
        {
            return;
        }

        else if(Input.GetKeyDown(KeyCode.Mouse0) && dialogueIsPlaying)
        {
            ContinueStory();
        }
    }

    public void Func_SpoonsButtonPressed()
    {
        cityAnimator.SetBool("SpoonsPressed", true);
        spoonsButton.interactable = false;

        EnterDialogueMode(spoonsNPCDialogue);
    }

    public void Func_SpoonsBackButtonPressed()
    {
        cityAnimator.SetBool("SpoonsPressed", false);
        spoonsButton.interactable = true;
    }

    #region Ink Dialogue Stuff - Tutorial used found in link Below
    //https://youtu.be/vY0Sk93YUhA
    
    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        ContinueStory();
    }

    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();
        }
        else
        {
            ExitDialogueMode();
        }
    }

    #endregion

    public void Func_TestButtonPress()
    {
        Debug.Log("BEEP");
    }
}
