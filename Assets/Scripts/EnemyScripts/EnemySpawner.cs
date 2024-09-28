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
    private Vector3 spawnPoint;

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
            RandomiseSpawnPoint();
            SpawnEnemy(GetEnemyToSpawn());
        }
    }

    private void ResetSpawnTimer()
    {
        spawnCountdown = spawnTimer;
    }

    private void RandomiseSpawnPoint()
    {
        spawnPoint.x = (transform.position.x + Random.Range(-2.0f, 2.0f));
        spawnPoint.y = (transform.position.y + Random.Range(-2.0f, 2.0f));
        spawnPoint.z = (transform.position.z + Random.Range(-2.0f, 2.0f));
    }

    private Enemy GetEnemyToSpawn()
    {
        enemyToSpawn = enemiesArray[Random.Range(0, enemiesArray.Length)];
        return enemyToSpawn;
    }

    private void SpawnEnemy(Enemy spawnEnemy)
    {
        Instantiate(spawnEnemy, spawnPoint, Quaternion.identity);
    }
}
