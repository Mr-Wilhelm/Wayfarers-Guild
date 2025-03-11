using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Book : MonoBehaviour
{
    [SerializeField] GameObject crystal;

    // Start is called before the first frame update
    public void DisableCrystal()
    {
        crystal.SetActive(false);
    }

    public void EnableCrystal() 
    {
        crystal.SetActive(true);
    }
}
