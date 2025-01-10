using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : EnemyParent
{
    private void Start()
    {
        Ship = GameObject.FindGameObjectWithTag("Ship");
        enemySpawner = GameObject.Find("EnemySpawner");
    }
}
