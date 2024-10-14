using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyParent : NetworkBehaviour
{
    [SerializeField]
    protected float moveSpeed;

    [SerializeField]
    protected float followRadius;

    [SerializeField]
    private GameObject Ship;

    [SerializeField]
    protected NavMeshAgent navMesh;

    [SerializeField]
    protected SphereCollider followSphere;

    [SerializeField]
    protected float destroyCountdown;

    [SerializeField]
    protected float destroyTimer;

    [SerializeField]
    protected bool isInRadius;

    public bool gameStarted;

    private GameObject enemySpawner;


    private void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
        followSphere = GetComponent<SphereCollider>();
        destroyCountdown = destroyTimer;
        Ship = GameObject.Find("Ship");
        enemySpawner = GameObject.Find("EnemySpawner");

        if(followSphere.isTrigger)
        {
            followSphere.radius = followRadius;
        }
    }

    private void Update()
    {
        navMesh.destination = Ship.transform.position;

        if (!isInRadius)
        {
            destroyCountdown -= Time.deltaTime;
        }

        if (destroyCountdown <= 0)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                Debug.Log("Calling despawn");
                enemySpawner.GetComponent<EnemySpawner>().DespawnEnemy(gameObject.GetComponent<Enemy>());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Entered Radius");
            isInRadius = true;
            destroyCountdown = destroyTimer;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Left radius");
            isInRadius = false;
        }
    }
}
