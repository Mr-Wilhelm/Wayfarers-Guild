using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class SCR_HubertoLogic : MonoBehaviour
{
    private SCR_Pathfinding enemyPathFinder;

    [SerializeField]
    private Vector3 moveTarget;

    [SerializeField]
    private int wanderRadius;

    [SerializeField]
    private Queue<Vector3> enemyPath = new Queue<Vector3>();

    private float navTolerance = 2;

    private Rigidbody rb;

    [SerializeField]
    private float moveSpeed = 10.0f;

    [SerializeField]
    private Vector3 currentDestination;

    [SerializeField]
    private Vector3 currentPos;

    [SerializeField]
    private LayerMask layerMask;

    private Bounds worldSize;

    // Start is called before the first frame update
    void Start()
    {
        enemyPathFinder = GameObject.Find("pathfinding").GetComponent<SCR_Pathfinding>();
        rb = GetComponent<Rigidbody>();
        worldSize = new Bounds(new Vector3(enemyPathFinder.endPos.x / 2, enemyPathFinder.endPos.y / 2, enemyPathFinder.endPos.z / 2), new Vector3(enemyPathFinder.endPos.x / 2, enemyPathFinder.endPos.y / 2, enemyPathFinder.endPos.z / 2));
        if (!GetComponent<NetworkTransform>().IsServer)
        {
            enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        

        
        if (enemyPath.Count > 0)   //if there are locations to move to
        {
            gameObject.transform.LookAt(currentDestination);
            currentPos = gameObject.transform.position; //gets the current pos of the object
            currentDestination = enemyPath.Peek();  //sets the current destination to the first element in the Queue
            if (Vector3.Distance(currentPos, currentDestination) > navTolerance) //if the object is not at the current object
            {
                gameObject.transform.position = Vector3.MoveTowards(currentPos, currentDestination, moveSpeed * Time.deltaTime);    //Move towards the first element in the list

            }
            else    //if the object has arrived at its target node
            {
                enemyPath.Dequeue();    //dequeue the first element
            }
        }
        else
        {
            if (moveTarget != new Vector3(-1, -1, -1))
            {
                UpdatePath();
            }
            
        }
    }
    private void UpdatePath()
    {
        var pathNodes = enemyPathFinder.FindPath(transform.position, moveTarget);    //find the path between the current pos and the target
        Vector3 prevPos = transform.position;
        if (pathNodes == null)
        {
            return;
        }
        foreach (var node in pathNodes)
        {
            Debug.DrawLine(prevPos, node, Color.red, 0.5f);
            prevPos = node;
        }

        enemyPath.Clear();  //clears the current path

        foreach (var nodePos in pathNodes)
        {
            enemyPath.Enqueue(nodePos); //instantiate a new queue for a new path
        }
    }

    private Vector3 newPosition()
    {
        for (var i = 0; i < 100; i++)
        {
            Vector3 candidate = gameObject.transform.position + (Random.insideUnitSphere * wanderRadius);

            if (!Physics.CheckSphere(candidate, 10, layerMask) && worldSize.Contains(candidate))
            {
                return candidate;
            }
        }
        return new Vector3(-1, -1, -1);
    }

}
