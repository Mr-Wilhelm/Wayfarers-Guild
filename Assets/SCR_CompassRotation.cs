using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_CompassRotation : MonoBehaviour
{
    [SerializeField] GameObject PortalObject;
    [SerializeField] Vector3 portalLocation;
    [SerializeField] GameObject crystal;
    [SerializeField] bool crystalParticlesEnabled = true;


    void Update()
    {
        //All of this is in update rather than start due to networking, Start is not running as intended.
        if (PortalObject == null)
        {
            Debug.Log("PortalObject aint doing it");
            PortalObject = GameObject.Find("Portal");
            portalLocation = PortalObject.transform.position;
        }
        if (crystal == null)
        {
            Debug.Log("Crystal no");
            crystal = this.gameObject.transform.GetChild(0).gameObject;
        }
        if (crystalParticlesEnabled) { crystal.transform.GetChild(1).gameObject.SetActive(true); }
        else { crystal.transform.GetChild(1).gameObject.SetActive(false); }



        //rotates the crystall to face the portal
        if (portalLocation != null && crystal != null)
        {
            crystal.transform.LookAt(portalLocation);
        }

    }
}
