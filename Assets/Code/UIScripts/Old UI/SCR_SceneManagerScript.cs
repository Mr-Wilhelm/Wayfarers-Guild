using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class SCR_SceneManagerScript : MonoBehaviour
{
    public void LoadMainScene()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        NetworkManager.Singleton.SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void LoadDefeatScene()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //SceneManager.LoadScene(1);
        NetworkManager.Singleton.SceneManager.LoadScene("DeathScene", LoadSceneMode.Single);
    }

    public void LoadWinScene()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //SceneManager.LoadScene(2);
        NetworkManager.Singleton.SceneManager.LoadScene("WinScene", LoadSceneMode.Single);
    }

    public void LoadCityScene()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        NetworkManager.Singleton.SceneManager.LoadScene("SCN_NewCityScene", LoadSceneMode.Single);
        //NetworkManager.Singleton.SceneManager.LoadScene("CityMenu", LoadSceneMode.Single);
    }

    public void ExitScene()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Debug.Log("No Dont do that");
            //Application.Quit();
        }

        //if(Input.GetKey(KeyCode.L))
        //{
        //    LoadCityScene();
        //}

        //if(Input.GetKey(KeyCode.R))
        //{
        //    LoadMainScene();
        //}
    }

}
