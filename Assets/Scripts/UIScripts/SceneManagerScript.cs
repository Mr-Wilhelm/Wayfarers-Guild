using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class SceneManagerScript : MonoBehaviour
{
    public void LoadMainScene()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(0);
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

    public void ExitScene()
    {
        Application.Quit();
    }

}
