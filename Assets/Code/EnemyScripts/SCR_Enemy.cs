using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class SCR_Enemy : MonoBehaviour
{
    [SerializeField]
    private SCR_Pathfinding enemyPathFinder;

    [SerializeField]
    private GameObject moveTarget;

    [SerializeField]
    private Queue<Vector3> enemyPath = new Queue<Vector3>();

    [SerializeField]
    private List<Vector3> dummyList = new List<Vector3>();

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
    private int selectedPrime;

    // Start is called before the first frame update
    void Start()
    {
        //moveTarget = GameObject.Find("target");
        enemyPathFinder = GameObject.Find("pathfinding").GetComponent<SCR_Pathfinding>();
        rb = GetComponent<Rigidbody>();

        Debug.DrawLine(gameObject.transform.position, moveTarget.transform.position, Color.green, 1000f);

        UpdatePath();

        selectedPrime = Random.Range(0, 4);

        switch (selectedPrime)
        {
            case 0:
                frameOffset = 181;
                break;
            case 1:
                frameOffset = 191;
                break;
            case 2:
                frameOffset = 193;
                break;
            case 3:
                frameOffset =197;
                break;
            case 4:
                frameOffset = 199;
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.LookAt(moveTarget.transform.position);
        frames++;

        if(frames % frameOffset == 0)
        {
            UpdatePath();
        }
        if (enemyPath.Count > 0)   //if there are locations to move to
        {
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
    }
    private void UpdatePath()
    {
        var pathNodes = enemyPathFinder.FindPath(currentPos, moveTarget.transform.position);    //find the path between the current pos and the target
        enemyPath.Clear();  //clears the current path

        foreach (var nodePos in pathNodes)
        {
            enemyPath.Enqueue(nodePos); //instantiate a new queue for a new path
        }
    }
}
