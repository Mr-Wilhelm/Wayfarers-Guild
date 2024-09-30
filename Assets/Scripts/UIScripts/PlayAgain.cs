using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayAgain : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;

    public void ReplayGame()
    {
        SceneManager.LoadScene(0);
        Destroy(canvas);
    }
}
