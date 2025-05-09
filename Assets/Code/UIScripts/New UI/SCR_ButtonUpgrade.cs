using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SCR_ButtonUpgrade : MonoBehaviour
{
    [SerializeField]
    public string upgradeName;

    [SerializeField]
    private string upgradeDescription;

    [SerializeField]
    private float upgradeCost;

    [SerializeField]
    private Image upgradeImage;

    [SerializeField]
    private SCR_Upgrades upgradeDisplay;

    private void Start()
    {
        upgradeDisplay = GameObject.Find("---UPGRADE UI---").GetComponent<SCR_Upgrades>();
        upgradeName = gameObject.name;

        switch (upgradeName)
        {
            case "ArmourUpgradeButton":
                upgradeDescription = "In The Outer Realms, you're likely to run into more than a couple creatures" +
                    "that want nothing more than to shred you to pieces." +
                    "Armor up with Hull Armor to ensure you make it through, and keep all your packages (and yourselves) in one piece.";
                upgradeCost = 100.0f;
                upgradeImage = GameObject.Find("ArmourUpgradeIcon").GetComponent<Image>();
                break;
            case "HealthUpgradeButton":
                upgradeDescription = "Health Upgrade Description";
                upgradeCost = 100.0f;
                upgradeImage = GameObject.Find("HealthUpgradeIcon").GetComponent<Image>();
                break;
            case "SonarUpgradeButton":
                upgradeDescription = "In the vast, unknowable expanses of The Outer Realms, it's imperative to know where you're going." +
                    " That's why we've made the GSS, or Geosonar system, to help you find your way!" +
                    " Simply press the button to send out a sonar ping (By purchasing this product you waive rights"+
                    " to sue us for any monster attacks caused by use of sonar equipment.)";
                upgradeCost = 100.0f;
                upgradeImage = GameObject.Find("SonarUpgradeIcon").GetComponent<Image>();
                break;
            case "AmmoUpgradeButton":
                upgradeDescription = "In the event that you find yourself swarmed by horrors with no escape," +
                    "there's no need to fear with this new multi-bolt magazine system! Now, you can hold up to 4 bolts at a time," +
                    " all ready to shoot one after the other, without that annoying manual reload in the way.";
                upgradeCost = 100.0f;
                upgradeImage = GameObject.Find("AmmoUpgradeIcon").GetComponent<Image>();
                break;
            case "ScopeUpgradeButton":
                upgradeDescription = "Sometimes, you just really need to shoot that tiny, far-off nightmare that just won't go away." +
                    " With this telescopic lens, we guarantee you probably won't miss your shot!* (All guarantees are non-legally binding)";
                upgradeCost = 100.0f;
                upgradeImage = GameObject.Find("ScopeUpgradeIcon").GetComponent<Image>();
                break;
            case "SpeedUpgradeButton":
                upgradeDescription = "Speed is essential in the work of a Wayfarer. But those pesky elemental engines just never quite seem fast enough." +
                    "The Acceleration Driver uses Arcane Particles to help make sure you reach top speed faster, and keep that top speed higher, so that you can keep your jobs!";
                upgradeCost = 50.0f;
                upgradeImage = GameObject.Find("SpeedUpgradeIcon").GetComponent<Image>();
                break;
            case "LightUpgradeButton":
                upgradeDescription = "This light may be small, but its wide, short-range beam can cut through The Outer Realms' fog like a knife through butter." +
                    " Great for those times when you're desperately lost and running late on your deliveries!";
                upgradeCost = 50.0f;
                upgradeImage = GameObject.Find("LightUpgradeIcon").GetComponent<Image>();
                break;
            default:
                return;

        }
    }

    public void GetButtonUpgrade()
    {
        upgradeDisplay.ShowUpgradesDescription();
        upgradeDisplay.descriptionToShow.text = upgradeDescription;
        upgradeDisplay.upgradeCostToShow.text = upgradeCost.ToString();
        upgradeDisplay.imageToShow.sprite = upgradeImage.sprite;
    }
}
