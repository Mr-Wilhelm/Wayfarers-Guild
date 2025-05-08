using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class SCR_Upgrades : NetworkBehaviour
{
    [SerializeField]
    private SCR_ButtonUpgrade buttonUpgrade;

    public string descriptionString;

    public TextMeshProUGUI descriptionToShow;

    public TextMeshProUGUI upgradeCostToShow;

    public Image imageToShow;

    private void Start()
    {
        buttonUpgrade = GameObject.Find("LightUpgradeButton").GetComponent<SCR_ButtonUpgrade>();
        imageToShow = GameObject.Find("Icon").GetComponent<Image>();
        descriptionToShow = GameObject.Find("UpgradeText").GetComponent<TextMeshProUGUI>();
        upgradeCostToShow = GameObject.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
    }
}
