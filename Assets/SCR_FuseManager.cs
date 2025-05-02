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

    public GameObject[] bridgeLights;
    public GameObject[] cargoHoldLights;
    public GameObject[] engineRoomLights;

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

    private void GetLightsOfType(string lightTag)
    {
        switch (lightTag)
        {
            case "bridgeLight":
                bridgeLights = GameObject.FindGameObjectsWithTag(lightTag);
                break;
            case "cargoHoldLight":
                cargoHoldLights = GameObject.FindGameObjectsWithTag(lightTag);
                break;
            case "engineRoomLight":
                engineRoomLights = GameObject.FindGameObjectsWithTag(lightTag);
                break;
            default:
                Debug.Log("Get lights of type switch statement broken");
                break;
        } 
    }

    public void DisableBridge()
    {
        print("Disabling bridge");
        DisableBridgeLights();
        bridgeStatusLight.GetComponent<MeshRenderer>().material = red;
    }

    public void DisableCargoHold()
    {
        DisableCargoHoldLights();
        cargoHoldStatusLight.GetComponent<MeshRenderer>().material = red;
    }

    public void DisableEngineRoom()
    {
        DisableEngineRoomLights();
        engineRoomStatusLight.GetComponent<MeshRenderer>().material = red;
    }

    private void DisableBridgeLights()
    {
        //GetLightsOfType("BridgeLight");
        foreach(GameObject lightOBJ in bridgeLights)
        {
            SetEmergencyLightValues(lightOBJ.GetComponent<Light>());
            print("Disabling a bridge light");
        }
    }

    private void DisableCargoHoldLights()
    {
        //GetLightsOfType("CargoHoldLight");
        foreach (GameObject lightOBJ in cargoHoldLights)
        {
            SetEmergencyLightValues(lightOBJ.GetComponent<Light>());
        }
    }

    private void DisableEngineRoomLights()
    {
        //GetLightsOfType("EngineRoomLight");
        foreach (GameObject lightOBJ in engineRoomLights)
        {
            SetEmergencyLightValues(lightOBJ.GetComponent<Light>());
        }
    }
}
