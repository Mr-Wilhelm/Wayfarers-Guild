using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class SCR_Upgrades : NetworkBehaviour
{
    [SerializeField]
    private string descriptionString;

    [SerializeField]
    private TextMeshProUGUI descriptionToShow;

    [SerializeField]
    private TextMeshProUGUI upgradeCostToShow;

    [SerializeField]
    private Image imageToShow;

    private void Start()
    {
        imageToShow = GameObject.Find("Icon").GetComponent<Image>();
        descriptionToShow = GameObject.Find("UpgradeText").GetComponent<TextMeshProUGUI>();
        upgradeCostToShow = GameObject.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
    }

    public void PressUpgradeButton()
    {
        
    }

    public void GetUpgradeInfo(string upgradeDescription, float upgradeCost, Image upgradeImage)
    {

    }
}
