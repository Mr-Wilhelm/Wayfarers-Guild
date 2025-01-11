using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SCR_Pathfinding : MonoBehaviour
{
    public Vector3[,,] navigationMatrix;



    private struct gridNode
    {
        public Vector3 position;

        public int forwardNeighbour, backNeighbour, rightNeighbour, leftNeighbour, upNeighbour, downNeighbour;

        public int traversalCost;



        public gridNode(Vector3 Position, int ForwardNeighbour, int BackNeighbour, int RightNeighbour, int LeftNeighbour, int UpNeighbour, int DownNeighbour, int TraversalCost)
        {
            this.position = Position;
            this.forwardNeighbour = ForwardNeighbour;
            this.backNeighbour = BackNeighbour;
            this.rightNeighbour = RightNeighbour;
            this.leftNeighbour = LeftNeighbour;
            this.upNeighbour = UpNeighbour;
            this.downNeighbour = DownNeighbour;
            this.traversalCost = TraversalCost;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PopulateWorld(100, 100, 100, 10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Populates the world with nodes
    /// </summary>
    /// <param name="x"> The Width of the world </param>
    /// <param name="y"> The Height of the world </param>
    /// <param name="z"> The Depth of the world </param>
    /// <param name="nodeSpacing"></param>
    private void PopulateWorld(float x, float y, float z, float nodeSpacing)
    {
        Debug.Log("Populating World");
        navigationMatrix = new Vector3
            [(int)Mathf.Floor(x / nodeSpacing),    //gets the number of nodes for the worlds width, x
            (int)Mathf.Floor(y / nodeSpacing),    //get the number of nodes for the worlds height, y
            (int)Mathf.Floor(z / nodeSpacing)];    //get the number of nodes for the worlds depth, z

        //iterate through the three dimensional array, going through width, then height, then depth (x, y ,z)
        for(int i = 0; i < navigationMatrix.GetLength(0); i++)            
        {
            Debug.Log("Iterated Through x");

            for (int j = 0; j < navigationMatrix.GetLength(1); j++)
            {
                Debug.Log("Iterated Through y");

                for (int k = 0; k < navigationMatrix.GetLength(2); k++)
                {
                    Debug.Log("Iterated Through z");
                    navigationMatrix[i, j, k] = new Vector3(i * nodeSpacing, j * nodeSpacing, k * nodeSpacing);
                    //return new Vector3(i * nodeSpacing, j * nodeSpacing, k * nodeSpacing);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (Vector3 pos in navigationMatrix)
        {
            Gizmos.DrawSphere(pos, 0.25f);
        }
        //Gizmos.DrawSphere(PopulateWorld(1000, 100, 2000, 10), 1.0f);
    }
}
