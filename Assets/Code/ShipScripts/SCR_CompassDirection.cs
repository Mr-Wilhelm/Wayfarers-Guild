using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_CompassDirection : MonoBehaviour
{
    /// <summary>
    /// portal GameObject
    /// </summary>
    [SerializeField] GameObject portalGO;

    /// <summary>
    /// The lifetime of the particles that are spawned, in this case used to see how far they travel
    /// </summary>
    [SerializeField] float particleEffectLength = 1.0f;

    private ParticleSystem compassParticles;

    private Vector3 portalPosition;
    private Vector3 portalDistance;
    private Vector3 portalDirection;


    void Start()
    {
        //Gets the portal GamObject if needed
        if (portalGO == null)
        {
            portalGO = GameObject.Find("Portal");
            if(portalGO == null)
            {
                Debug.LogError("Portal not found");
            }
        }
        //Gets the Portal Position (assumes it doesnt move)
        portalPosition = portalGO.transform.position;
        
        //Gets the particle system that emits the particles
        compassParticles = this.GetComponent<ParticleSystem>();
        var compassParticlesMain = compassParticles.main;
        compassParticlesMain.startLifetime = particleEffectLength;
    }

    void Update()
    {
        //calulates the direction to be facing
        portalDistance = portalPosition - this.transform.position;
        portalDirection = portalDistance.normalized;

        //rotates compass to face the portal
        RotateCompass(portalDirection);

    }

    /// <summary>
    /// Rotates this GameObject to face the direction given
    /// </summary>
    /// <param name="Direction"></param>
    private void RotateCompass(Vector3 Direction)
    {
        Quaternion lookDir = Quaternion.LookRotation(Direction);
        this.gameObject.transform.rotation = lookDir;
    }

}
