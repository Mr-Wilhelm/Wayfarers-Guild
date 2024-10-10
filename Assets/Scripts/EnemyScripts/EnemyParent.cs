using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyParent : MonoBehaviour
{
    [SerializeField]
    protected float moveSpeed;

    [SerializeField]
    protected float followRadius;

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

    private void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
        followSphere = GetComponent<SphereCollider>();
        destroyCountdown = destroyTimer;

        if(followSphere.isTrigger)
        {
            followSphere.radius = followRadius;
        }
    }

    private void Update()
    {
        navMesh.destination = GameObject.FindGameObjectWithTag("Player").transform.position;

        if (!isInRadius)
        {
            destroyCountdown -= Time.deltaTime;
        }

        if (destroyCountdown <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("Entered Radius");
            isInRadius = true;
            destroyCountdown = destroyTimer;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("Left radius");
            isInRadius = false;
        }
    }
}
