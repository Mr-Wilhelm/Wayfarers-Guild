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

    // Start is called before the first frame update
    void Start()
    {
        //moveTarget = GameObject.Find("target");
        enemyPathFinder = GameObject.Find("pathfinding").GetComponent<SCR_Pathfinding>();
        rb = GetComponent<Rigidbody>();

        Debug.DrawLine(gameObject.transform.position, moveTarget.transform.position, Color.green, 1000f);

        foreach (var nodePos in enemyPathFinder.FindPath(gameObject.transform.position, moveTarget.transform.position))
        {
            enemyPath.Enqueue(nodePos);
        }
    }

    // Update is called once per frame
    void Update()
    {


        if (enemyPath.Count != 0)   //if there are locations to move to
        {
            currentPos = gameObject.transform.position;
            currentDestination = enemyPath.Peek();

            if (Vector3.Distance(currentPos, currentDestination) > navTolerance) //if the object is not at the current object
            {
                Debug.Log(Vector3.Distance(currentPos, currentDestination));
                gameObject.transform.position = Vector3.MoveTowards(currentPos, currentDestination, moveSpeed * Time.deltaTime);
            }
            else
            {
                Debug.Log("Dequeuing");
                enemyPath.Dequeue();
            }
        }
        else
        {
            Debug.Log("List Empty");
        }

    }

    void UpdatePath()
    {
        enemyPath.Clear();  //clears the current path

        

    }
}
