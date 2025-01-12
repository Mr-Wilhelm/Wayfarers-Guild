using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SCR_Pathfinding : MonoBehaviour
{
    /// <summary>
    /// Makes a 3d array (matrix) of structs, with a vector3 of their position.
    /// Allows for functions to be called on each independent struct object in the matrix.
    /// 
    /// USING THE LEFT HAND COORDINATE SYSTEM - positive Z is in front of the origin, Positive X is to the right.
    /// 
    /// </summary>
    /// 
    public gridNode[,,] navigationMatrix;

    public Vector3 startPos;
    public Vector3 endPos;
    public float nodeSize = 1.0f;

    private List<gridNode> openList = new List<gridNode>();
    private List<gridNode> closedList = new List<gridNode>();
    public struct gridNode
    {
        public Vector3 position;

        public int gCost, hCost;
        public int fCost => gCost + hCost;  //the => makes it read only?

        public Vector3 previousNodeIndex;

        //absolute chonker of a struct constructor
        public gridNode(Vector3 Position, int GCost, int HCost, Vector3 PreviousNodeIndex)
        {
            this.position = Position;
            this.gCost = GCost;
            this.hCost = HCost;
            this.previousNodeIndex = PreviousNodeIndex;
        }
        /// <param name="x"> Gets the X position of the obejct in the matrix </param>
        /// <param name="y"> Gets the Y position of the object in the matrix </param>
        /// <param name="z"> Gets the Z position of the object in the matrix </param>
        /// 
        public Vector3[,,] GetAdjacentNodes(int x, int y, int z, int x_length, int y_length, int z_length)
        {
            // Each section is a row along the x-axis. (left - middle -right)
            Vector3[,,] neighbour = new Vector3[3, 3, 3];

            // y = 0 (bottom layer)
            // z = 0 (front layer)
            //bottom back left, middle, right
            neighbour[0, 0, 0] = new Vector3(x - 1, y - 1, z - 1);    // x = 0 (left)         
            neighbour[1, 0, 0] = new Vector3(x, y - 1, z - 1);            // x = 1 (middle)   
            neighbour[2, 0, 0] = new Vector3(x + 1, y - 1, z - 1);          // x = 2 (right)
                

            // y = 0 (bottom layer)
            // z = 1 (middle layer)
            //bottom middle left, middle, right
            neighbour[0, 0, 1] = new Vector3(x - 1, y - 1, z);    // x = 0 (left)
            neighbour[1, 0, 1] = new Vector3(x, y - 1, z);        // x = 1 (middle)
            neighbour[2, 0, 1] = new Vector3(x + 1, y - 1, z);    // x = 2 (right)

            // y = 0 (bottom layer)
            // z = 2 (back layer)
            //bottom front left, middle, right
            neighbour[0, 0, 2] = new Vector3(x - 1, y - 1, z + 1);    // x = 0 (left)
            neighbour[1, 0, 2] = new Vector3(x, y - 1, z + 1);        // x = 1 (middle)
            neighbour[2, 0, 2] = new Vector3(x + 1, y - 1, z + 1);    // x = 2 (right)

            // y = 1 (middle layer)
            // z = 0 (front layer)
            //middle back left, middle, right
            neighbour[0, 1, 0] = new Vector3(x - 1, y, z - 1);    // x = 0 (left)
            neighbour[1, 1, 0] = new Vector3(x, y, z - 1);        // x = 1 (middle)
            neighbour[2, 1, 0] = new Vector3(x + 1, y, z - 1);    // x = 2 (right)

            // y = 1 (middle layer)
            // z = 1 (middle layer)
            //middle middle left, middle, right
            neighbour[0, 1, 1] = new Vector3(x - 1, y, z);    // x = 0 (left)
            neighbour[1, 1, 1] = new Vector3(x, y, z);        // x = 1 (middle)
            neighbour[2, 1, 1] = new Vector3(x + 1, y, z);    // x = 2 (right)

            // y = 1 (middle layer)
            // z = 2 (back layer)
            //middle front left, middle, right
            neighbour[0, 1, 2] = new Vector3(x - 1, y, z + 1);    // x = 0 (left)
            neighbour[1, 1, 2] = new Vector3(x, y, z + 1);        // x = 1 (middle)
            neighbour[2, 1, 2] = new Vector3(x + 1, y, z + 1);    // x = 2 (right)

            // y = 2 (top layer)
            // z = 0 (front layer)
            //top back left, middle, right
            neighbour[0, 2, 0] = new Vector3(x - 1, y + 1, z - 1);    // x = 0 (left)
            neighbour[1, 2, 0] = new Vector3(x, y + 1, z - 1);        // x = 1 (middle)
            neighbour[2, 2, 0] = new Vector3(x + 1, y + 1, z - 1);    // x = 2 (right)

            // y = 2 (top layer)
            // z = 1 (middle layer)
            //top middle left, middle, right
            neighbour[0, 2, 1] = new Vector3(x - 1, y + 1, z);    // x = 0 (left)
            neighbour[1, 2, 1] = new Vector3(x, y + 1, z);        // x = 1 (middle)
            neighbour[2, 2, 1] = new Vector3(x + 1, y + 1, z);    // x = 2 (right)

            // y = 2 (top layer)
            // z = 2 (back layer)
            //top front left, middle, right
            neighbour[0, 2, 2] = new Vector3(x - 1, y + 1, z + 1);    // x = 0 (left)
            neighbour[1, 2, 2] = new Vector3(x, y + 1, z + 1);        // x = 1 (middle)
            neighbour[2, 2, 2] = new Vector3(x + 1, y + 1, z + 1);    // x = 2 (right)

            //Boundary check for the neighbour nodes
            for (int i = 0; i < neighbour.GetLength(0); i++)
            {
                for (int j = 0; j < neighbour.GetLength(1); j++)
                {
                    for (int k = 0; k < neighbour.GetLength(2); k++)
                    {
                        if (neighbour[i, j, k].x >= x_length || neighbour[i, j, k].x < 0)
                        {
                            neighbour[i, j, k] = new Vector3(-1, -1, -1);
                        }
                        if (neighbour[i, j, k].y >= y_length || neighbour[i, j, k].y < 0)
                        {
                            neighbour[i, j, k] = new Vector3(-1, -1, -1);
                        }
                        if (neighbour[i, j, k].z >= z_length || neighbour[i, j, k].z < 0)
                        {
                            neighbour[i, j, k] = new Vector3(-1, -1, -1);
                        }
                    }
                }
            }

            return neighbour;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PopulateWorld(1000, 500, 1000);

        foreach (Vector3 v in navigationMatrix[0, 0, 0].GetAdjacentNodes(0, 0, 0, 100, 50, 100))
        {
            Debug.Log(v);
        }
        
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
    private void PopulateWorld(float x, float y, float z)
    {
        Debug.Log("Populating World");
        navigationMatrix = new gridNode
            [(int)Mathf.Floor(x / nodeSize),    //gets the number of nodes for the worlds width, x
            (int)Mathf.Floor(y / nodeSize),    //get the number of nodes for the worlds height, y
            (int)Mathf.Floor(z / nodeSize)];    //get the number of nodes for the worlds depth, z

        //iterate through the three dimensional array, going through width, then height, then depth (x, y ,z)
        for(int i = 0; i < navigationMatrix.GetLength(0); i++)            
        {
            for (int j = 0; j < navigationMatrix.GetLength(1); j++)
            {
                for (int k = 0; k < navigationMatrix.GetLength(2); k++)
                {
                    navigationMatrix[i, j, k].position = new Vector3(i * nodeSize, j * nodeSize, k * nodeSize;
                    navigationMatrix[i, j, k].GetAdjacentNodes(i, j, k, 1000, 500, 1000);    //calls the function with the current indexes as the parameters
                }
            }
        }
    }

    private List<Vector3> FindPath(Vector3 startPoint, Vector3 endPoint)
    {
        //get the start and end points via the parameters passed.
        gridNode startNode = navigationMatrix[(int)(startPoint.x / nodeSize), (int)(startPoint.y / nodeSize), (int)(startPoint.z / nodeSize)];
        gridNode endNode = navigationMatrix[(int)(endPoint.x / nodeSize), (int)(endPoint.y / nodeSize), (int)(endPoint.z / nodeSize)];

        //cleaning lists
        openList.Clear();
        closedList.Clear();

        //Setting the g and h cost of the start node by getting the distance between the positions of the start and end nodes.
        startNode.gCost = 0;
        startNode.hCost = (int)Vector3.Distance(startNode.position, endNode.position);

        openList.Add(startNode);    //add the start node to the open list

        while (openList.Count > 0)  //while there are nodes in the open list
        {
            gridNode currentNode = openList[0]; //current node is the first entry in the list (currently the only one, and the one it is at
            foreach (var node in openList)  //iterate through the open list
            {
                //compare fCost values, if they're the same, compare gCost values to see if the node the iteration is on, is less than the node the enemy is currently at
                if (node.fCost < currentNode.fCost || node.fCost == currentNode.fCost && node.gCost < currentNode.gCost)
                {
                    currentNode = node;
                }
            }

            //remove the current node from the open list and add it to the closed list, since it has now been visited
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if(currentNode.position == endNode.position)
            {
                return RemakePath(currentNode);
            }
        }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    foreach (gridNode pos in navigationMatrix)
    //    {
    //        Gizmos.DrawSphere(pos.position, 0.25f);
    //    }
    //    //Gizmos.DrawSphere(PopulateWorld(1000, 100, 2000, 10), 1.0f);
    //}
}

    private List<Vector3> RemakePath(gridNode currentNode)
    {
        throw new NotImplementedException();
    }
}

