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

    // Start is called before the first frame update
    void Start()
    {
        moveTarget = GameObject.Find("target");
        enemyPathFinder = GameObject.Find("pathfinding").GetComponent<SCR_Pathfinding>();
        rb = GetComponent<Rigidbody>();

        Debug.DrawLine(gameObject.transform.position, moveTarget.transform.position, Color.green, 1000f);

        foreach (var nodePos in enemyPathFinder.FindPath(gameObject.transform.position, moveTarget.transform.position))
        {
            enemyPath.Enqueue(nodePos);
            Debug.Log(nodePos);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (enemyPath.Count != 0)    //if the queue is not finished
        {
            if (Vector3.Distance(gameObject.transform.position, enemyPath.Peek()) > navTolerance)
            {
                //rb.AddForce((gameObject.transform.position - enemyPath.Peek()).normalized * moveSpeed);
                rb.AddForce((enemyPath.Peek() - gameObject.transform.position).normalized * moveSpeed, ForceMode.Force);
                //Debug.Log((enemyPath.Peek() - gameObject.transform.position).normalized);
            }
            else
            {
                enemyPath.Dequeue();
                Debug.Log("DEqueud");
            }
        }

    }
}
