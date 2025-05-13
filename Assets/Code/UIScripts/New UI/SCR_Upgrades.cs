using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class SCR_Upgrades : NetworkBehaviour
{
    [SerializeField]
    private SCR_NewUiManager uiManager;

    [Header("ChosenUpgradeStats")]

    public SCR_ButtonUpgrade buttonUpgrade;

    public string descriptionString;

    public TextMeshProUGUI descriptionToShow;

    public TextMeshProUGUI upgradeCostToShow;

    public float upgradeCost;

    public Image imageToShow;

    public Image selectedUpgrade;

    [Header("Pages")]
    [SerializeField]
    private Image repairsPage;

    [SerializeField]
    private Image upgradeInfoPage;

    [SerializeField]
    private Image upgradeListPage;

    [SerializeField]
    private GameObject purchaseButton;

    [SerializeField]
    private SCR_PlayerDataHandler playerDataHandler;

    [Header("RepairsPage")]
    [SerializeField]
    public TextMeshProUGUI currentShipHealthText;

    [SerializeField]
    public TextMeshProUGUI maxShipHealthText;

    [SerializeField]
    public TextMeshProUGUI repairCostText;

    private void Start()
    {
        uiManager = GameObject.Find("NewUICanvas").GetComponent<SCR_NewUiManager>();
        playerDataHandler = GameObject.Find("PlayerDataHandler").GetComponent<SCR_PlayerDataHandler>();

        buttonUpgrade = GameObject.Find("LightUpgradeButton").GetComponent<SCR_ButtonUpgrade>();
        imageToShow = GameObject.Find("Icon").GetComponent<Image>();
        descriptionToShow = GameObject.Find("UpgradeText").GetComponent<TextMeshProUGUI>();
        upgradeCostToShow = GameObject.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
        selectedUpgrade = GameObject.Find("SelectedUpgrade").GetComponent<Image>();
        purchaseButton = GameObject.Find("Purchase");

        repairsPage = GameObject.Find("ShipRepairsPage").GetComponent<Image>();
        upgradeInfoPage = GameObject.Find("UpgradesDescription").GetComponent<Image>();
        upgradeListPage = GameObject.Find("UpgradesListPage").GetComponent<Image>();

        currentShipHealthText = GameObject.Find("CurrentShipHealthText").GetComponent<TextMeshProUGUI>();
        maxShipHealthText = GameObject.Find("ShipMaxHealthText").GetComponent<TextMeshProUGUI>();
        repairCostText = GameObject.Find("RepairCostText").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        currentShipHealthText.text = playerDataHandler.shipHealthGlobal.Value.ToString();
        maxShipHealthText.text = playerDataHandler.GetMaxHealth().ToString();
        repairCostText.text = uiManager.repairCost.ToString();
    }

    //button functions

    public void ShowRepairsPage()
    {
        repairsPage.enabled = true;
        upgradeInfoPage.enabled = false;
        upgradeListPage.enabled = false;
        selectedUpgrade.enabled = false;

        buttonUpgrade.enabled = false;
        imageToShow.enabled = false;
        descriptionToShow.enabled = false;
        upgradeCostToShow.enabled = false;
        purchaseButton.SetActive(false);
    }
    public void ShowUpgradesDescription()
    {
        repairsPage.enabled = false;
        upgradeInfoPage.enabled = true;
        upgradeListPage.enabled = false;

        buttonUpgrade.enabled = true;
        imageToShow.enabled = true;
        descriptionToShow.enabled = true;
        upgradeCostToShow.enabled = true;
        selectedUpgrade.enabled = true;
        purchaseButton.SetActive(true);
    }
    public void ShowUpgradesList()
    {
        repairsPage.enabled = false;
        upgradeInfoPage.enabled = false;
        upgradeListPage.enabled = true;

        buttonUpgrade.enabled = false;
        imageToShow.enabled = false;
        descriptionToShow.enabled = false;
        upgradeCostToShow.enabled = false;
        selectedUpgrade.enabled = false;
        purchaseButton.SetActive(false);
    }

    public void PurchaseSelectedUpgrade()
    {
        if(playerDataHandler.playerMoney.Value >= upgradeCost)
        {
            if(buttonUpgrade.isOneTimeUpgrade && buttonUpgrade.upgradeBought.Value == false)    //if its a one time upgrade and hasn't been bought
            {
                switch(buttonUpgrade.upgradeName)
                {
                    case "SonarUpgradeButton":
                        Debug.Log("Sonar Upgrade Bought");
                        playerDataHandler.SonarUpgradeBought.Value = true;
                        break;
                    case "AmmoUpgradeButton":
                        playerDataHandler.AmmoUpgradeBought.Value = true;
                        Debug.Log("Ammo Upgrade Bought");
                        break;
                    case "ScopeUpgradeButton":
                        playerDataHandler.ScopeUpgradeBought.Value = true;
                        Debug.Log("Scope Upgrade Bought");
                        break;
                    case "LightUpgradeButton":
                        playerDataHandler.LightUpgradeBought.Value = true;
                        Debug.Log("Light Upgrade Bought");
                        break;
                    default:
                        break;
                }

                playerDataHandler.playerMoney.Value -= upgradeCost;
                buttonUpgrade.upgradeBought.Value = true;
                Debug.Log("Upgrade Cost, " + upgradeCost + "New Total amount of money is, " + playerDataHandler.playerMoney.Value);

            }
            else if(!buttonUpgrade.isOneTimeUpgrade)    //if its not a one time upgrade and has bought less than three times
            {
                if (buttonUpgrade.upgradeName == "ArmourUpgradeButton" && playerDataHandler.armourUpgradesBought < 3)
                {
                    playerDataHandler.playerMoney.Value -= upgradeCost;
                    playerDataHandler.armourUpgradesBought += 1;
                }
                else if(buttonUpgrade.upgradeName == "HealthUpgradeButton" && playerDataHandler.healthUpgradesBought < 3)
                {
                    playerDataHandler.playerMoney.Value -= upgradeCost;
                    playerDataHandler.healthUpgradesBought += 1;
                }
                else if(buttonUpgrade.upgradeName == "SpeedUpgradeButton" && playerDataHandler.speedUpgradesBought < 3)
                {
                    playerDataHandler.playerMoney.Value -= upgradeCost;
                    playerDataHandler.speedUpgradesBought += 1;
                }
            }
        }
        else
        {
            Debug.Log("Did not purchase");
        }
    }
    public void PurchaseRepairs()
    {
        uiManager.RepairShip();
    }    
}
