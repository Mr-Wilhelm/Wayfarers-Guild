using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_LoadedFromMainMenuCheck : MonoBehaviour
{

    [SerializeField] private string menuSceneToLoad;
    [SerializeField] public string playSceneToLoad;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        playSceneToLoad = SceneManager.GetActiveScene().name;

        //network manager is loaded from main menu, if it does not exist you know the scene was not loaded from main menu
        if (GameObject.Find("NetworkManager") == null)
        {
            //Loads main menu if demo scene was not loaded from main menu already
            Debug.Log("No network manager found, loading main menu");
            SceneManager.LoadScene(menuSceneToLoad, LoadSceneMode.Single);
        }
    }
}
