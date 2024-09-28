using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private Enemy[] enemiesArray;

    [SerializeField]
    private Enemy enemyToSpawn;

    [SerializeField]
    private float spawnCountdown;

    [SerializeField]
    private float spawnTimer;

    [SerializeField]
    private float spawnPoint;

    private void Start()
    {
        spawnCountdown = spawnTimer;
    }

    private void Update()
    {
        spawnCountdown -= Time.deltaTime;

        if (spawnCountdown <= 0)
        {
            Debug.Log("Timer at zero");
            ResetSpawnTimer();
        }
    }

    private void ResetSpawnTimer()
    {
        spawnCountdown = spawnTimer;
    }

    private void RandomiseSpawnPoint()
    {

    }

    private Enemy GetEnemyToSpawn()
    {
        enemyToSpawn = enemiesArray[Random.Range(0, enemiesArray.Length)];
        return enemyToSpawn;
    }

    private void SpawnEnemy()
    {

    }


}
