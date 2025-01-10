using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class SCR_EnemySpawner : NetworkBehaviour
{
    [SerializeField]
    private SCR_Enemy[] enemiesArray;

    [SerializeField]
    private SCR_Enemy enemyToSpawn;

    [SerializeField]
    private float spawnCountdown;

    [SerializeField]
    private float spawnTimer;

    [SerializeField]
    private Vector3 spawnPoint;

    public bool startSpawning;

    private void Start()
    {
        spawnCountdown = spawnTimer;
        startSpawning = false;
    }

    private void Update()
    {
        if(!IsServer)
        {
            //if you're not the network server, dont spawn things
            return;
        }


        if (startSpawning)
        {
            spawnCountdown -= Time.deltaTime;

            if (spawnCountdown <= 0)
            {
                ResetSpawnTimer();
                RandomiseSpawnPoint();

                //Spawns an enemy using the value returned from the GetEnemyToSpawn() function
                SpawnEnemy(GetEnemyToSpawn());
            }
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

    /// <summary>
    /// Gets a random enemy from the array
    /// sets it as the enemy to spawn
    /// returns that value
    /// </summary>
    /// <returns></returns>
    private SCR_Enemy GetEnemyToSpawn()
    {
        enemyToSpawn = enemiesArray[Random.Range(0, enemiesArray.Length)];
        return enemyToSpawn;
    }

    /// <summary>
    /// Gets the spawnEnemy value from the GetEnemyToSpawn() function
    /// Spawns it at a random position around the spawner
    /// </summary>
    /// <param name="spawnEnemy"></param>
    private void SpawnEnemy(SCR_Enemy spawnEnemy)
    {
        var Instance = Instantiate(spawnEnemy, spawnPoint, Quaternion.identity);
        var instanceNetworkObject = Instance.GetComponent<NetworkObject>();
        instanceNetworkObject.Spawn(true);
    }

    public void DespawnEnemy(SCR_Enemy despawnEnemy)
    {
        try
        {
            despawnEnemy.gameObject.GetComponent<NetworkObject>().Despawn();
            Debug.Log("Done the despawn");
        }
        catch
        {
            Debug.Log("Failed to despawn");
        }
    }
}
