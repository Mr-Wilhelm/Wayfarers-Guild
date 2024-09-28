using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        navMesh.destination = GameObject.FindGameObjectWithTag("Player").transform.position;
    }

}
