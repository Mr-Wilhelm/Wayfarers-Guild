using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SCR_NetworkedShipHealth : NetworkBehaviour
{
    public NetworkVariable<float> shipHealth = new NetworkVariable<float>();
    public int maxHealth = 100;

    public float lerpSpeed = 3.5f;

    public Image healthBar;

    // Start is called before the first frame update
    void Start()
    {
        shipHealth.OnValueChanged += ShipHealth_OnValueChange;
        if (IsHost) { shipHealth.Value = GameObject.FindGameObjectWithTag("PlayerDataHandler").GetComponent<SCR_PlayerDataHandler>().shipHealthGlobal.Value; } else { healthBar.fillAmount = (shipHealth.Value / maxHealth); }
    }

    public void changeHealth(float changeBy)
    {
        if (IsHost) { shipHealth.Value += changeBy; }
        
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = (Mathf.Lerp(healthBar.fillAmount*100, (shipHealth.Value / maxHealth)*100, Time.deltaTime * lerpSpeed) / 100);
    }

    override public void OnDestroy()
    {
        if (IsHost) { GameObject.FindGameObjectWithTag("PlayerDataHandler").GetComponent<SCR_PlayerDataHandler>().shipHealthGlobal.Value = shipHealth.Value; }
        base.OnDestroy();
    }

    private void ShipHealth_OnValueChange(float previousValue, float newValue)
    {
        if (IsHost) { if (shipHealth.Value <= 0f) { GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SCR_SceneManagerScript>().LoadDefeatScene(); } }

        //healthBar.fillAmount = (shipHealth.Value / maxHealth);
    }
}
