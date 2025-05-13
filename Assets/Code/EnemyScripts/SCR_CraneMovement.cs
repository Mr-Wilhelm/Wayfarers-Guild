using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class SCR_CraneMovement : MonoBehaviour
{
    [SerializeField]
    private SCR_Pathfinding enemyPathFinder;

    [SerializeField]
    private GameObject moveTarget;

    [SerializeField]
    private Queue<Vector3> enemyPath = new Queue<Vector3>();

    [SerializeField]
    private float navTolerance = 2;

    [SerializeField]
    private Rigidbody rb;

    [SerializeField]
    private float moveSpeed = 10.0f;

    [SerializeField]
    private Vector3 currentDestination;

    [SerializeField]
    private Vector3 currentPos;

    [SerializeField]
    private int frames;

    [SerializeField]
    private int frameOffset;

    [SerializeField]
    private SCR_ShipMovement shipVariables;

    [SerializeField]
    private float damage = 5;

    [SerializeField]
    private bool move = true;

    [SerializeField]
    private float wanderRadius;

    [SerializeField]
    private Vector3 wanderLocation;

    [SerializeField]
    private Vector3 startPosition;

    [SerializeField]
    private SphereCollider attackRadius;

    private void Start()
    {
        moveTarget = GameObject.Find("MothTargetPoint");
        enemyPathFinder = GameObject.Find("pathfinding").GetComponent<SCR_Pathfinding>();
        rb = GetComponent<Rigidbody>();

        Debug.DrawLine(gameObject.transform.position, moveTarget.transform.position, Color.green, 1000f);

        startPosition = gameObject.transform.position;
    }

    private void Update()
    {
        if (!move) { return; }
        gameObject.transform.LookAt(moveTarget.transform.position);
        frames++;

        if(frames % frameOffset == 0)
        {
            UpdatePath();
        }
        if(enemyPath.Count == 0)
        {
            //get a new destination to wander to when reaching the wander destination
            GetWanderDestination();
            UpdatePath();
            return;
        }

        Vector3 currentPos = transform.position;
        Vector3 destination = enemyPath.Peek();

        //pretty much same movement code as before
        if (Vector3.Distance(currentPos, destination) > navTolerance)
        {
            transform.position = Vector3.MoveTowards(currentPos, destination, moveSpeed * Time.deltaTime);
            transform.LookAt(destination);

        }
        else
        {
            enemyPath.Dequeue();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(startPosition, wanderRadius);
    }

    private void GetWanderDestination()
    {
        Vector3 wanderOffset = Random.insideUnitSphere * wanderRadius;  //get random vec3 within a set radius
        wanderLocation = startPosition + wanderOffset; //set wander location to the random position relative to start position
        Debug.DrawLine(transform.position, wanderLocation, Color.yellow, 2f);
    }

    private void UpdatePath()
    {
        var pathNodes = enemyPathFinder.FindPath(transform.position, wanderLocation);
        Vector3 prevPos = startPosition + transform.position;
        if(pathNodes == null)
        {
            return;
        }
        foreach(var node in pathNodes)
        {
            Debug.DrawLine(prevPos, node, Color.red, 0.5f);
            prevPos = node;
        }

        enemyPath.Clear();

        foreach(var nodePos in pathNodes)
        {
            enemyPath.Enqueue(nodePos);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //TODO: Make crane target airship
        //      Stop the random wandering
        //      Set destination to airship until death or trigger exit
    }
}
