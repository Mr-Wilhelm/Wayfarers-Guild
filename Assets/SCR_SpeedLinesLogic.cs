using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_SpeedLinesLogic : MonoBehaviour
{

    private ParticleSystem speedLines;

    // Start is called before the first frame update
    void Start()
    {
        speedLines = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        var emission = speedLines.emission;
        emission.rateOverTime = (GetComponentInParent<Rigidbody>().velocity.magnitude/18) - 10;
    }
}
