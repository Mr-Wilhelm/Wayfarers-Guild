using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SCR_ButtonUpgrade : MonoBehaviour
{
    [SerializeField]
    private string upgradeName;

    [SerializeField]
    private string upgradeDescription;

    [SerializeField]
    private float upgradeCost;

    [SerializeField]
    private Image upgradeImage;

    private void Start()
    {
        upgradeName = gameObject.name;

        switch (upgradeName)
        {
            case "ArmourUpgradeButton":
                upgradeDescription = "Armour Upgrade Description";
                upgradeCost = 100.0f;
                upgradeImage = GameObject.Find("ArmourUpgradeIcon").GetComponent<Image>();
                break;
            case "HealthUpgradeButton":
                upgradeDescription = "Armour Upgrade Description";
                upgradeCost = 100.0f;
                upgradeImage = GameObject.Find("ArmourUpgradeIcon").GetComponent<Image>();
                break;
            case "SonarUpgradeButton":
                upgradeDescription = "Armour Upgrade Description";
                upgradeCost = 100.0f;
                upgradeImage = GameObject.Find("ArmourUpgradeIcon").GetComponent<Image>();
                break;
            case "AmmoUpgradeButton":
                upgradeDescription = "Armour Upgrade Description";
                upgradeCost = 100.0f;
                upgradeImage = GameObject.Find("ArmourUpgradeIcon").GetComponent<Image>();
                break;
            case "ScopeUpgradeButton":
                upgradeDescription = "Armour Upgrade Description";
                upgradeCost = 100.0f;
                upgradeImage = GameObject.Find("ArmourUpgradeIcon").GetComponent<Image>();
                break;
            case "SpeedUpgradeButton":
                upgradeDescription = "Armour Upgrade Description";
                upgradeCost = 100.0f;
                upgradeImage = GameObject.Find("ArmourUpgradeIcon").GetComponent<Image>();
                break;
            case "LightUpgradeButton":
                upgradeDescription = "Armour Upgrade Description";
                upgradeCost = 100.0f;
                upgradeImage = GameObject.Find("ArmourUpgradeIcon").GetComponent<Image>();
                break;
            default:
                return;

        }
    }
}
