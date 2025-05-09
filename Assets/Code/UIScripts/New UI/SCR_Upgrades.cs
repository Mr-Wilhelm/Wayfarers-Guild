using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class SCR_Upgrades : NetworkBehaviour
{
    [Header("ChosenUpgradeStats")]
    [SerializeField]
    private SCR_ButtonUpgrade buttonUpgrade;

    public string descriptionString;

    public TextMeshProUGUI descriptionToShow;

    public TextMeshProUGUI upgradeCostToShow;

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

    private void Start()
    {
        buttonUpgrade = GameObject.Find("LightUpgradeButton").GetComponent<SCR_ButtonUpgrade>();
        imageToShow = GameObject.Find("Icon").GetComponent<Image>();
        descriptionToShow = GameObject.Find("UpgradeText").GetComponent<TextMeshProUGUI>();
        upgradeCostToShow = GameObject.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
        selectedUpgrade = GameObject.Find("SelectedUpgrade").GetComponent<Image>();
        purchaseButton = GameObject.Find("Purchase");

        repairsPage = GameObject.Find("ShipRepairsPage").GetComponent<Image>();
        upgradeInfoPage = GameObject.Find("UpgradesDescription").GetComponent<Image>();
        upgradeListPage = GameObject.Find("UpgradesListPage").GetComponent<Image>();
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
}
