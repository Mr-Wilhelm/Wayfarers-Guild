using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Enemy : SCR_EnemyParent
{
    private void Start()
    {
        Ship = GameObject.FindGameObjectWithTag("Ship");
        enemySpawner = GameObject.Find("SCR_EnemySpawner");
    }
}
