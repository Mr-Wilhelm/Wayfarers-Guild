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

    [Header("Upgrade Costs")]
    [SerializeField]
    private float healthCost = 100f;

    [SerializeField]
    private float armourCost = 100f;

    [SerializeField]
    private float speedCost = 50f;

    [SerializeField]
    private float lightCost = 50f;

    [SerializeField]
    private float sonarCost = 100f;

    [SerializeField]
    private float scopeCost = 100f;

    [SerializeField]
    private float ammoCost = 100f;

    private void Awake()
    {
        playerDataHandler = GameObject.Find("PlayerDataHandler").GetComponent<SCR_PlayerDataHandler>();
        if (playerDataHandler == null )
        {
            Debug.LogError("No Player Data Handler Found!!!!!!");
        }
    }

    public override void OnNetworkSpawn()
    {
        if(playerDataHandler == null)
        {
            playerDataHandler = GameObject.Find("PlayerDataHandler").GetComponent<SCR_PlayerDataHandler>();
        }
    }
    private void Start()
    {
        uiManager = GameObject.Find("NewUICanvas").GetComponent<SCR_NewUiManager>();
        

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
        Debug.Log("Money is: " + playerDataHandler.playerMoney.Value);
        if(playerDataHandler.playerMoney.Value >= upgradeCost)
        {
            float chosenUpgradeCost = 0f;
            if (buttonUpgrade.isOneTimeUpgrade && buttonUpgrade.upgradeBought.Value == false)    //if its a one time upgrade and hasn't been bought
            {

                switch (buttonUpgrade.upgradeName)
                {
                    case "SonarUpgradeButton":
                        if(playerDataHandler.playerMoney.Value >= sonarCost)
                        {
                            chosenUpgradeCost = sonarCost;
                            SyncSonarServerRpc();
                        }
                        break;
                    case "AmmoUpgradeButton":
                        if(playerDataHandler.playerMoney.Value >= ammoCost)
                        {
                            chosenUpgradeCost = ammoCost;
                            SyncAmmoServerRpc();
                        }
                        break;
                    case "ScopeUpgradeButton":
                        if (playerDataHandler.playerMoney.Value >= scopeCost)
                        {
                            chosenUpgradeCost = scopeCost;
                            SyncScopeServerRpc();
                        }
                        break;
                    case "LightUpgradeButton":
                        if (playerDataHandler.playerMoney.Value >= lightCost)
                        {
                            chosenUpgradeCost = lightCost;
                            SyncLightServerRpc();
                        }
                        break;
                    default:
                        break;
                }
            }
            else if(!buttonUpgrade.isOneTimeUpgrade)    //if its not a one time upgrade and has bought less than three times
            {
                if (buttonUpgrade.upgradeName == "ArmourUpgradeButton" && playerDataHandler.armourUpgradesBought < 3)
                {
                    if(playerDataHandler.playerMoney.Value >= armourCost)
                    {
                        chosenUpgradeCost = armourCost;
                        SyncArmourServerRpc();
                    }

                }
                else if(buttonUpgrade.upgradeName == "HealthUpgradeButton" && playerDataHandler.healthUpgradesBought < 3)
                {
                    if (playerDataHandler.playerMoney.Value >= healthCost)
                    {
                        chosenUpgradeCost = healthCost;
                        SyncHealthServerRpc();
                    }
                }
                else if(buttonUpgrade.upgradeName == "SpeedUpgradeButton" && playerDataHandler.speedUpgradesBought < 3)
                {
                    if (playerDataHandler.playerMoney.Value >= speedCost)
                    {
                        chosenUpgradeCost = speedCost;
                        SyncSpeedServerRpc();
                    }
                }
            }

            SyncMoneyCountServerRpc(chosenUpgradeCost);
        }
        else
        {
            Debug.Log("Did not purchase");
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void SyncSonarServerRpc()
    {
        Debug.Log("Sonar Upgrade Bought");
        playerDataHandler.SonarUpgradeBought.Value = true;
        Debug.Log("Sonar Upgrade Bought");
    }
    [ServerRpc(RequireOwnership = false)]
    public void SyncAmmoServerRpc()
    {
        Debug.Log("Ammo Upgrade Bought");
        playerDataHandler.AmmoUpgradeBought.Value = true;
        Debug.Log("Ammo Upgrade Bought");
    }
    [ServerRpc(RequireOwnership = false)]
    public void SyncScopeServerRpc()
    {
        Debug.Log("Scope Upgrade Bought");
        playerDataHandler.ScopeUpgradeBought.Value = true;
        Debug.Log("Scope Upgrade Bought");
    }
    [ServerRpc(RequireOwnership = false)]
    public void SyncLightServerRpc()
    {
        Debug.Log("Light Upgrade Bought");
        playerDataHandler.LightUpgradeBought.Value = true;
        Debug.Log("Light Upgrade Bought");
    }
    [ServerRpc(RequireOwnership = false)]
    public void SyncHealthServerRpc()
    {
        Debug.Log("Health Upgrade Bought");
        playerDataHandler.armourUpgradesBought += 1;
        Debug.Log("Health Upgrade Bought");
    }
    [ServerRpc(RequireOwnership = false)]
    public void SyncArmourServerRpc()
    {
        Debug.Log("Armour Upgrade Bought");
        playerDataHandler.healthUpgradesBought += 1;
        Debug.Log("Armour Upgrade Bought");
    }
    [ServerRpc(RequireOwnership = false)]
    public void SyncSpeedServerRpc()
    {
        Debug.Log("Speed Upgrade Bought");
        playerDataHandler.speedUpgradesBought += 1;
        Debug.Log("Speed Upgrade Bought");
    }
    [ServerRpc(RequireOwnership = false)]
    public void SyncMoneyCountServerRpc(float selectCost)
    {
        Debug.Log("Syncing Money");
        playerDataHandler.playerMoney.Value -= selectCost;
        buttonUpgrade.upgradeBought.Value = true;
        Debug.Log("Upgrade Cost, " + selectCost + "New Total amount of money is, " + playerDataHandler.playerMoney.Value);
    }

    public void PurchaseRepairs()
    {
        uiManager.RepairShip();
    }
}
