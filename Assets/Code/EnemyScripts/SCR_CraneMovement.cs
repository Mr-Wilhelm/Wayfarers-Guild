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

    private void Start()
    {
        moveTarget = GameObject.Find("MothTargetPoint");
        enemyPathFinder = GameObject.Find("pathfinding").GetComponent<SCR_Pathfinding>();
        rb = GetComponent<Rigidbody>();

        Debug.DrawLine(gameObject.transform.position, moveTarget.transform.position, Color.green, 1000f);
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
        if(enemyPath.Count > 0)
        {
            currentPos = gameObject.transform.position; //get current position
            currentDestination = enemyPath.Peek();  //set current destination to first object in queue
            if(Vector3.Distance(currentPos, currentDestination) > navTolerance) //if not at destination
            {
                gameObject.transform.position = Vector3.MoveTowards(currentPos, currentDestination, moveSpeed * Time.deltaTime);    //move towards first element in list
            }
            else //if at destination
            {
                enemyPath.Dequeue();
            }
        }
    }

    private void UpdatePath()
    {
        var pathNodes = enemyPathFinder.FindPath(transform.position, moveTarget.transform.position);
        Vector3 prevPos = transform.position;
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
}
