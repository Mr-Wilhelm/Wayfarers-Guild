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
    /// USING THE LEFT HAND COORDINATE SYSTEM - positive Z is in front of the origin, Positive X is to the right.#
    /// TODO: There is a logic issue in the first while loop. POssibly in the g, f or h cost
    /// 
    /// </summary>
    /// 
    public gridNode[,,] navigationMatrix;

    public Vector3 startPos;
    public Vector3 endPos;
    public float nodeSize = 10f;

    private List<gridNode> openList = new List<gridNode>();
    private List<gridNode> closedList = new List<gridNode>();

    public int iterator, iterator2;

    //public GameObject target;
    public struct gridNode
    {
        public Vector3 position;

        public int gCost, hCost;
        //public int fCost => gCost + hCost;  //the => makes it read only?

        public int fCost;

        public Vector3 previousNodePosition;
        public Vector3 index;

        //absolute chonker of a struct constructor
        public gridNode(Vector3 Position, int GCost, int HCost, Vector3 previousNodePosition, Vector3 Index)
        {
            this.position = Position;
            this.gCost = GCost;
            this.hCost = HCost;
            this.fCost = gCost + hCost;
            this.previousNodePosition = previousNodePosition;
            this.index = Index;
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
                            neighbour[i, j, k] = new Vector3(-1, -1 -1);
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

    public bool done = false;

    private void Awake()
    {
        Debug.Log("Awake");
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
    }

    private void OnEnable()
    {
        please();
    }

    public void please()
    {
        if (!done)
        {
            Debug.Log("Start");
            PopulateWorld(40, 40, 40);
            if (FindPath(new Vector3(1, 1, 1), new Vector3(3, 3, 3)) == null)
            {
                Debug.Log("fuck");
            }
            else
            {
                foreach (Vector3 pos in FindPath(new Vector3(1, 1, 1), new Vector3(3, 3, 3)))
                {
                    Debug.Log(pos);
                }
            }
            done = true;
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
                    navigationMatrix[i, j, k].position = new Vector3(i * nodeSize, j * nodeSize, k * nodeSize);
                    navigationMatrix[i, j, k].index = new Vector3(i, j, k);
                    Debug.Log("Wabungus");
                }
            }
        }
    }

    private List<Vector3> FindPath(Vector3 startPoint, Vector3 endPoint)
    {
        //get the start and end points via the parameters passed.
        gridNode startNode = navigationMatrix[(int)(startPoint.x), (int)(startPoint.y), (int)(startPoint.z)];
        gridNode endNode = navigationMatrix[(int)(endPoint.x), (int)(endPoint.y), (int)(endPoint.z)];

        //cleaning lists
        openList.Clear();
        closedList.Clear();

        //Setting the g and h cost of the start node by getting the distance between the positions of the start and end nodes.
        startNode.gCost = 0;
        startNode.hCost = (int)Vector3.Distance(startNode.position, endNode.position);

        openList.Add(startNode);    //add the start node to the open list

        while (openList.Count > 0)  //while there are nodes in the open list
        {
            iterator++;
            Debug.Log("219");
            gridNode currentNode = openList[0]; //current node is the first entry in the list (currently the only one, and the one it is at
            Debug.Log("221");
            foreach (var node in openList)  //iterate through the open list
            {
                Debug.Log("224");
                //compare fCost values, if they're the same, compare gCost values to see if the node the iteration is on, is less than the node the enemy is currently at
                if (node.fCost < currentNode.fCost || node.fCost == currentNode.fCost && node.gCost < currentNode.gCost)
                {
                    Debug.Log("228");
                    currentNode = node;
                }
            }

            //remove the current node from the open list and add it to the closed list, since it has now been visited
            Debug.Log("234");
            openList.Remove(currentNode);
            Debug.Log("236");
            closedList.Add(currentNode);
            Debug.Log("238");

            if(currentNode.position == endNode.position)
            {
                Debug.Log("242");
                return RemakePath(currentNode, startPoint); //remake the path when you reach the next node
                
            }

            Debug.Log("247");
            foreach (var neighbourPos in currentNode.GetAdjacentNodes   //iterates through all adjacent nodes
                ((int)currentNode.index.x,
                (int)currentNode.index.y,
                (int)currentNode.index.z,
                navigationMatrix.GetLength(0), navigationMatrix.GetLength(1), navigationMatrix.GetLength(2)))
            {
                Debug.Log("254");
                //gets the neighbour node
                if (neighbourPos.x == -1)
                {
                    continue;
                }
                Debug.Log("260");
                gridNode neighbourNode = navigationMatrix[(int)(neighbourPos.x), (int)(neighbourPos.y), (int)(neighbourPos.z)];
                Debug.Log("262");

                //Checks if the node is in the closedList, continuing if so.
                //if (closedList.Exists(n => n.position == neighbourNode.position))
                if (closedList.Contains(neighbourNode))
                    continue;

                Debug.Log("269");
                //otherwise get the estimated gCost to reach the neighbour node from the start node
                int estimatedGCost = currentNode.gCost + (int)Vector3.Distance(currentNode.position, neighbourNode.position);
                Debug.Log("272");

                //if the neighbour is not already in the open list, or if the estimated cost is lower than the current gCost
                if (!openList.Contains(neighbourNode) || estimatedGCost < neighbourNode.gCost)
                {
                    Debug.Log("277");
                    neighbourNode.gCost = estimatedGCost; //update the gCost of the neighbour
                    Debug.Log("279");
                    neighbourNode.hCost = (int)Vector3.Distance(neighbourNode.position, endNode.position);//get the hcost of the new neighbour
                    Debug.Log("281");
                    neighbourNode.previousNodePosition = currentNode.position;//update the previous node position to that of the current one
                    Debug.Log("283");

                    if (!openList.Contains(neighbourNode))    //if the node isn't already in the list, add it
                    {
                        Debug.Log("287");
                        openList.Add(neighbourNode);    //add the neighbour node to the open list

                    }
                }
            }
            if(iterator >= 100)
            {
                foreach (var wabung in openList)
                {
                    Debug.Log(wabung.index);
                }
                Debug.Log(openList.Count);
                Debug.Log("Breaking at first while loop");
                break;
            }
        }
        return null;    //No Path found
    }

    private List<Vector3> RemakePath(gridNode currentNode, Vector3 originalNode)
    {
        Debug.Log("299");
        List<Vector3> newPath = new List<Vector3>();    //make a new list for the new path
        Debug.Log("301");
        while (currentNode.index != originalNode)    //iterate through the path from end to start (backwards)
        {
            iterator2++;
            Debug.Log("304");
            newPath.Add(currentNode.position);  //add currentNode.position to the new path
            Debug.Log("306");
            currentNode = navigationMatrix[(int)(currentNode.previousNodePosition.x), (int)(currentNode.previousNodePosition.y), (int)(currentNode.previousNodePosition.z)];   //move to the previous node
            Debug.Log("308");

            if(iterator2 >= 100)
            {
                Debug.Log("Break at 2nd while loop");
                break;
            }
        }

        Debug.Log("311");
        //revert the path so its now front to back again :)
        newPath.Reverse();
        Debug.Log("314");
        //return the path
        return newPath;
    }
}

