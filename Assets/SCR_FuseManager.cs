using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_FuseManager : MonoBehaviour
{
    [SerializeField] private GameObject bridgeStatusLight;
    [SerializeField] private GameObject cargoHoldStatusLight;
    [SerializeField] private GameObject engineRoomStatusLight;

    [SerializeField] private Material red;
    [SerializeField] private Material green;

    [SerializeField] private Color lightColor;
    [SerializeField] private float lightTemperature;
    [SerializeField] private float lux;
    [SerializeField] private float radius;

    [SerializeField] private Color emergencyLightColor;
    [SerializeField] private float emergencyLightTemperature;
    [SerializeField] private float emergencyLux;
    [SerializeField] private float emergencyRadius;

    public GameObject[] Lights;
    private bool lightsOff;

    private void SetEmergencyLightValues(Light lightToChange)
    {
        lightToChange.color = emergencyLightColor;
        lightToChange.colorTemperature = emergencyLightTemperature;
        lightToChange.intensity = emergencyLux;
        lightToChange.range = emergencyRadius;
    }

    private void SetStandardLightValues(Light lightToChange)
    {
        lightToChange.color = lightColor;
        lightToChange.colorTemperature = lightTemperature;
        lightToChange.intensity = lux;
        lightToChange.range = radius;
    }

    public void EnableLights()
    {
        Debug.Log("TRYING TO ENABLE LIGHTS");
        lightsOff = false;
        bridgeStatusLight.GetComponent<MeshRenderer>().material = green;
        cargoHoldStatusLight.GetComponent<MeshRenderer>().material = green;
        engineRoomStatusLight.GetComponent<MeshRenderer>().material = green;

        foreach (GameObject lightOBJ in Lights)
        {
            SetStandardLightValues(lightOBJ.GetComponent<Light>());
        }
    }

    public void DisableBridge()
    {
        print("Disabling bridge");
        DisableLights();
        bridgeStatusLight.GetComponent<MeshRenderer>().material = red;
    }

    public void DisableCargoHold()
    {
        DisableLights();
        cargoHoldStatusLight.GetComponent<MeshRenderer>().material = red;
    }

    public void DisableEngineRoom()
    {
        DisableLights();
        engineRoomStatusLight.GetComponent<MeshRenderer>().material = red;
    }

    private void DisableLights()
    {
        if (lightsOff) { return; }
        lightsOff = true;
        foreach (GameObject lightOBJ in Lights)
        {
            SetEmergencyLightValues(lightOBJ.GetComponent<Light>());
        }
    }
}
