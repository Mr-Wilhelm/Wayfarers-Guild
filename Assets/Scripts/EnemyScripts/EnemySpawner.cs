using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private Enemy[] enemiesToSpawn;

    [SerializeField]
    private float spawnTimer;

    [SerializeField]
    private float spawnPoint;
}
