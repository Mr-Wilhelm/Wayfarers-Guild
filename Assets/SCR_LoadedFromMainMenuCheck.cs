using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_LoadedFromMainMenuCheck : MonoBehaviour
{

    [SerializeField] private string sceneToLoad;
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("NetworkManager") == null)
        {
            Debug.Log("No network manager found, loading main menu");
            SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
        }
        else
        {
            Debug.Log("Network manager found, proceeding");
        }
    }
}
