using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SCR_NewUiManager : MonoBehaviour
{
    [SerializeField]
    private Button spoonsButton;

    private void Start()
    {
        spoonsButton = GameObject.Find("BUTTON_Spoons").GetComponent<Button>();
    }

    public void Func_SpoonsButtonPressed()
    {
        Debug.Log("Spoons Button Pressed");
    }
}
