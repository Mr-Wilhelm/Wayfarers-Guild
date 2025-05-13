using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_CraneAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetFloat("FlightOffset", Random.Range(0.0f, 1.0f));
    }
}
