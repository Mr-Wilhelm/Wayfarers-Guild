using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_CompassRotation : MonoBehaviour
{
    [SerializeField] GameObject PortalGO;
    [SerializeField] Vector3 PortalLocation;
    [SerializeField] GameObject Crystal;


    void Update()
    {
        //All of this is in update rather than start due to networking, Start is not running as intended.
        if (PortalGO == null)
        {
            PortalGO = GameObject.Find("Portal");
            PortalLocation = PortalGO.transform.position;
        }
        if (Crystal == null)
        {
            Crystal = this.gameObject.transform.GetChild(0).gameObject;
        }



        //rotates the crystall to face the portal
        if (PortalLocation != null && Crystal != null)
        {
            Crystal.transform.LookAt(PortalLocation);
        }

    }
}
