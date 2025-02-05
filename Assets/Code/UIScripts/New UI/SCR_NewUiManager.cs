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
        //dialogue system variable assignment
    }

    public void Func_SpoonsButtonPressed()
    {
        cityAnimator.SetBool("SpoonsPressed", true);
        spoonsButton.interactable = false;

        //Invoke(EnterDialogueMode(spoonsNPCDialogue), 2.0f); it no likey this line my brain is exploding
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
        currentStory = new Story(inkJSON.text); //sets the current story to the variable
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);  //activates the dialogue panel
        audioSource.enabled = true;

        ContinueStory();    //Loads the next line of the story
    }

    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        audioSource.enabled = false;
    }

    private void ContinueStory()
    {
        List<string> tags = currentStory.currentTags;   //gets tags from the current story
        Debug.Log(tags.Count);

        if (currentStory.canContinue)
        {
            //Stops the current text typing from playing.
            //This fixes a bug where text overlaps from different dialogues
            if (typeTextCoroutine != null)
            {
                StopCoroutine(typeTextCoroutine);
            }

            typeTextCoroutine = StartCoroutine(TypeText(currentStory.Continue())); //set text for the current line
                                                                                   //display dialogue choices
            DisplayChoices();   //shows button choices
        }
        else
        {
            ExitDialogueMode();
        }
    }

    /// <summary>
    /// Takes the string, in this case the text of the current story
    /// Convert it to an array of Chars
    /// Iterate throug the array, adding to it
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            audioSource.clip = textSound;
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }

            yield return new WaitForSeconds(10.0f * Time.deltaTime);
        }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;  //gets current choices from the ink file

        if (currentChoices.Count > choices.Length)
        {
            //unity is set up to support up to three choices so far, lemme know if you want more
            Debug.LogError("More choies were given than the Ui can support, Will made this, so ask him for help if necessary. Number of choices given:" + currentChoices.Count);
        }

        int index = 0;

        //enable the choice buttons for the amount of current choices from the ink story
        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }



        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false); //go through the remaining choices the UI supports and make sure they're hidden.
        }
    }

    public void MakeChoice(int choiceIndex)
    {
        //the parameter is passed on the button press in the unity editor,
        //in the section where you assign functions to buttons
        currentStory.ChooseChoiceIndex(choiceIndex);
    }

    #endregion

    public void Func_TestButtonPress()
    {
        Debug.Log("BEEP");
    }
}
