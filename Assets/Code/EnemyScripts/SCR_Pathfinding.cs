using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public float nodeSize;

    public List<gridNode> openList = new List<gridNode>();
    public List<gridNode> closedList = new List<gridNode>();

    public int iterator, iterator2;

    //public GameObject target;
    public struct gridNode
    {
        public Vector3 position;

        public int gCost, hCost;
        //public int fCost => gCost + hCost;  //the => makes it read only?

        //public int fCost;

        public Vector3 previousNodeIndex;
        public Vector3 index;

        //absolute chonker of a struct constructor
        public gridNode(Vector3 Position, int GCost, int HCost, Vector3 PreviousNodeIndex, Vector3 Index)
        {
            this.position = Position;
            this.gCost = GCost;
            this.hCost = HCost;
            //this.fCost = gCost + hCost;
            this.previousNodeIndex = PreviousNodeIndex;
            this.index = Index;
        }

        public int GetFCost()
        {
            return gCost + hCost;
        }

        /// <param name="x"> Gets the X position of the obejct in the matrix </param>
        /// <param name="y"> Gets the Y position of the object in the matrix </param>
        /// <param name="z"> Gets the Z position of the object in the matrix </param>
        /// 
        public Vector3[,,] GetAdjacentNodes(int x, int y, int z, int x_length, int y_length, int z_length, float NodeSize, gridNode[,,] NavigationMatrix)
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
            neighbour[1, 1, 1] = new Vector3(-1, -1, -1);        // x = 1 (middle)
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
                        if (Physics.CheckSphere(NavigationMatrix[(int)neighbour[i,j,k].x, (int)neighbour[i, j, k].y, (int)neighbour[i, j, k].z].position, NodeSize, 12))
                        {
                            neighbour[i, j, k] = new Vector3(-1, -1, -1);
                        }

                    }
                }
            }
            

            return neighbour;
        }
    }

    private void Awake()
    {
        PopulateWorld(500, 150, 500);
    }

    // Start is called before the first frame update
    void Start()
    {
       
        //Vector3 A = new Vector3(3, 15, 24);
        //Vector3 B = new Vector3(90, 28, 64);

        //if (FindPath(A, B) == null)
        //{
        //    Debug.Log("fuck");
        //}
        //else
        //{
        //    Vector3 prevPos = A;
        //    Debug.DrawLine(A, B, Color.blue, 1000f);
        //    foreach (Vector3 pos in FindPath(A, B))
        //    {
        //        //Debug.Log(pos);
        //        Debug.DrawLine(prevPos, pos, Color.red, 1000f);
        //        prevPos = pos;
        //    }
        //}
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
                }
            }
        }
    }

    public List<Vector3> FindPath(Vector3 startPoint, Vector3 endPoint)
    {
        Debug.Log(new Vector3((int)MathF.Floor(startPoint.x / nodeSize), (int)MathF.Floor(startPoint.y / nodeSize), (int)MathF.Floor(startPoint.z / nodeSize)));
        Debug.Log(new Vector3((int)MathF.Floor(endPoint.x / nodeSize), (int)MathF.Floor(endPoint.y / nodeSize), (int)MathF.Floor(startPoint.z / nodeSize)));

        //get the start and end points via the parameters passed.
        gridNode startNode = navigationMatrix[(int)MathF.Floor(startPoint.x / nodeSize), (int)MathF.Floor(startPoint.y / nodeSize), (int)MathF.Floor(startPoint.z / nodeSize)];
        gridNode endNode = navigationMatrix[(int)MathF.Floor(endPoint.x / nodeSize), (int)MathF.Floor(endPoint.y / nodeSize), (int)MathF.Floor(endPoint.z / nodeSize)];

        //cleaning lists
        openList.Clear();
        closedList.Clear();

        //Setting the g and h cost of the start node by getting the distance between the positions of the start and end nodes.
        startNode.gCost = 0;
        startNode.hCost = (int)Vector3.Distance(startNode.position, endNode.position);
        startNode.previousNodeIndex = startNode.index;


        openList.Add(startNode);    //add the start node to the open list


        while (openList.Count > 0)  //while there are nodes in the open list
        {
            iterator++;
            gridNode currentNode = openList[0]; //current node is the first entry in the list (currently the only one, and the one it is at
            foreach (var node in openList)  //iterate through the open list
            {
                
                //compare fCost values, if they're the same, compare gCost values to see if the node the iteration is on, is less than the node the enemy is currently at
                if (node.GetFCost() < currentNode.GetFCost() || node.GetFCost() == currentNode.GetFCost() && node.gCost < currentNode.gCost)
                {
                    currentNode = node;
                }
            }

            //remove the current node from the open list and add it to the closed list, since it has now been visited
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if(currentNode.position == endNode.position)
            {
                return RemakePath(currentNode, startNode.index); //remake the path when you reach the next node
                
            }
            //Debug.Log("---------------" + currentNode.index + "---------------");
            foreach (var neighbourPos in currentNode.GetAdjacentNodes   //iterates through all adjacent nodes
                ((int)currentNode.index.x,
                (int)currentNode.index.y,
                (int)currentNode.index.z,
                navigationMatrix.GetLength(0), navigationMatrix.GetLength(1), navigationMatrix.GetLength(2),
                nodeSize, navigationMatrix))
            {
                //Debug.Log("neighbourPos : " +neighbourPos + " / - currentIndex : " + currentNode.index);
                //gets the neighbour node
                if ((int)neighbourPos.x == -1)
                {
                    continue;
                }
                gridNode neighbourNode = navigationMatrix[(int)(neighbourPos.x), (int)(neighbourPos.y), (int)(neighbourPos.z)];

                //Checks if the node is in the closedList, continuing if so.
                //if (closedList.Exists(n => n.position == neighbourNode.position))
                if (closedList.Contains(neighbourNode))
                    continue;

                //otherwise get the estimated gCost to reach the neighbour node from the start node
                int estimatedGCost = currentNode.gCost + (int)Vector3.Distance(currentNode.position, neighbourNode.position);

                //if the neighbour is not already in the open list, or if the estimated cost is lower than the current gCost
                if (!openList.Contains(neighbourNode) || estimatedGCost < neighbourNode.gCost)
                {
                    
                    neighbourNode.gCost = estimatedGCost; //update the gCost of the neighbour
                    neighbourNode.hCost = (int)Vector3.Distance(neighbourNode.position, endNode.position);//get the hcost of the new neighbour
                    neighbourNode.previousNodeIndex = currentNode.index;//update the previous node position to that of the current one
                    //Debug.Log("###############################");
                    //Debug.Log("neighbourNode.index : " + neighbourNode.index);
                    //Debug.Log("currentNode.index : " + currentNode.index);
                    navigationMatrix[(int)neighbourNode.index.x, (int)neighbourNode.index.y, (int)neighbourNode.index.z] = neighbourNode;

                    if (!openList.Contains(neighbourNode))    //if the node isn't already in the list, add it
                    {
                        openList.Add(neighbourNode);    //add the neighbour node to the open list

                    }
                }
            }
            //if(iterator >= 10000000)
            //{
            //    foreach (var wabung in openList)
            //    {
            //        //Debug.Log("-----------------");
            //        //Debug.Log("index : "+wabung.index);
                    
            //        //Debug.Log("hCost : " + wabung.hCost);
            //        //Debug.Log("gCost : " + wabung.gCost);
            //        //Debug.Log("fCost : " + wabung.GetFCost());

            //    }
            //    //Debug.Log(openList.Count);
            //    Debug.Log("Breaking at first while loop");
            //    break;
            //}
        }
        return null;    //No Path found
    }

    private List<Vector3> RemakePath(gridNode currentNode, Vector3 originalNode)
    {
        //Debug.Log("/////////////////////");
        //foreach (var wabung in openList)
        //{
        //    Debug.Log("index : " + wabung.index);
        //}
        //Debug.Log("vvvvvvvvvvvvvvvvvvvvvv");
        //foreach (var wabung in closedList)
        //{
        //    Debug.Log("index : " + wabung.index);
        //    Debug.Log("prev index : " + wabung.previousNodeIndex);
        //}

        //Debug.Log("++++++++++++++++++++++++++++++++++++++++");
        List<Vector3> newPath = new List<Vector3>();    //make a new list for the new path
        //Debug.Log("originalNode index : " + originalNode);
        iterator2 = 0;
        while (currentNode.index != originalNode)    //iterate through the path from end to start (backwards)
        {
            
            //Debug.Log("currentNode index : "+currentNode.index);
            //Debug.Log("currentNode previousNodeIndex : " + currentNode.previousNodeIndex);
            iterator2++;
            newPath.Add(currentNode.position);  //add currentNode.position to the new path
            currentNode = navigationMatrix[(int)(currentNode.previousNodeIndex.x), (int)(currentNode.previousNodeIndex.y), (int)(currentNode.previousNodeIndex.z)];   //move to the previous node
            //Debug.Log(currentNode.index);


            if(iterator2 >= 1000)
            {
                Debug.Log("Break at 2nd while loop");
                break;
            }
        }

        //revert the path so its now front to back again :)
        newPath.Reverse();
        //return the path
        return newPath;
    }

    void OnDrawGizmos()
    {
        if(navigationMatrix != null)
        {
            Gizmos.color = Color.yellow;
            foreach (gridNode pos in navigationMatrix)
            {
                Gizmos.DrawSphere(pos.position, nodeSize / 10);
            }
        }
    }
}

