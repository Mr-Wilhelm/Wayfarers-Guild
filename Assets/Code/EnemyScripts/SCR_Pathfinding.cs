using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SCR_Pathfinding : MonoBehaviour
{
    /// <summary>
    /// Makes a 3d array (matrix) of structs, with a vector3 of their position.
    /// Allows for functions to be called on each independent struct object in the matrix.
    /// </summary>
    public gridNode[,,] navigationMatrix;

    public Vector3 startPos;
    public Vector3 endPos;
    public float nodeSize;

    public int iterator, iterator2;

    public LayerMask layerMask;

    public GameObject debugPrefab;

    [SerializeField]
    private Vector4 nodeDebugColourPassable;

    [SerializeField]
    private Vector4 nodeDebugColourImpassable;



    //public GameObject target;
    public struct gridNode
    {
        public Vector3 position;

        public int gCost, hCost;

        //public int fCost;

        public Vector3 previousNodeIndex;
        public Vector3 index;
        public bool passable;

        public Vector3[,,] neighbour;

        //absolute chonker of a struct constructor
        public gridNode(Vector3 Position, int GCost, int HCost, Vector3 PreviousNodeIndex, Vector3 Index, bool Passable, Vector3[,,] Neighbour)
        {
            this.position = Position;
            this.gCost = GCost;
            this.hCost = HCost;
            //this.fCost = gCost + hCost;
            this.previousNodeIndex = PreviousNodeIndex;
            this.index = Index;
            this.passable = Passable;

            this.neighbour = Neighbour;
        }

        public int GetFCost()
        {
            return gCost + hCost;
        }
        public gridNode assignNeighbours(int x, int y, int z, int x_length, int y_length, int z_length)
        {
            Vector3[,,] assignedNeighbours = new Vector3[3, 3, 3];

            for (int offsetX = -1; offsetX <= 1; offsetX++)
            {
                for (int offsetY = -1; offsetY <= 1; offsetY++)
                {
                    for (int offsetZ = -1; offsetZ <= 1; offsetZ++)
                    {
                        int newX = x + offsetX;
                        int newY = y + offsetY;
                        int newZ = z + offsetZ;

                        //checks if the new position is in the matrix
                        if (newX >= 0 && newX < x_length && newY >= 0 && newY < y_length && newZ >= 0 && newZ < z_length)
                        {
                            //store the position
                            assignedNeighbours[offsetX + 1, offsetY + 1, offsetZ + 1] = new Vector3(newX, newY, newZ);
                        }
                        else
                        {
                            assignedNeighbours[offsetX + 1, offsetY + 1, offsetZ + 1] = new Vector3(-1, -1, -1);
                        }                        
                    }
                }
            }
            assignedNeighbours[1, 1, 1] = new Vector3(-1, -1, -1);

            return new gridNode(this.position, this.gCost, this.hCost, this.previousNodeIndex, this.index, this.passable, assignedNeighbours);
        }
    }

    private void Awake()
    {
        PopulateWorld(500, 100, 500);
        Debug.Log("World Populated.");
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

        Vector3[] directions = new Vector3[]
        {
            #region basic directions
            // basic directions, up, down, left, right, forwards, backwards
            (Vector3.up).normalized,
            (Vector3.down).normalized,
            (Vector3.left).normalized,
            (Vector3.right).normalized,
            (Vector3.forward).normalized,
            (Vector3.back).normalized,
            #endregion

            #region top layer
            (Vector3.up + Vector3.right).normalized,   //up right
            (Vector3.up + Vector3.left).normalized,   //up left
            (Vector3.up + Vector3.forward).normalized,   //up forward
            (Vector3.up + Vector3.back).normalized,   //up back

            (Vector3.up + Vector3.right + Vector3.forward).normalized, //up right forward
            (Vector3.up + Vector3.left + Vector3.forward).normalized, //up left forward
            (Vector3.up + Vector3.right + Vector3.back).normalized, //up right backward
            (Vector3.up + Vector3.left + Vector3.back).normalized, //up left backward

            #endregion

            #region middle layer
            (Vector3.right + Vector3.forward).normalized,   //middle right forward
            (Vector3.left + Vector3.forward).normalized,   //middle left forward
            (Vector3.right + Vector3.back).normalized,   //middle right backward
            (Vector3.left + Vector3.back).normalized,   //middle left backward
            #endregion

            #region bottom layer
            (Vector3.down + Vector3.right).normalized,   //down right
            (Vector3.down + Vector3.left).normalized,   //down left
            (Vector3.down + Vector3.forward).normalized,   //down forward
            (Vector3.down + Vector3.back).normalized,   //down back

            (Vector3.down + Vector3.right + Vector3.forward).normalized, //down right forward
            (Vector3.down + Vector3.left + Vector3.forward).normalized, //down left forward
            (Vector3.down + Vector3.right + Vector3.back).normalized, //down right backward
            (Vector3.down + Vector3.left + Vector3.back).normalized, //down left backward
            #endregion
        };

        //iterate through the three dimensional array, going through width, then height, then depth (x, y ,z)
        for (int i = 0; i < navigationMatrix.GetLength(0); i++)            
        {
            for (int j = 0; j < navigationMatrix.GetLength(1); j++)
            {
                for (int k = 0; k < navigationMatrix.GetLength(2); k++)
                {
                    navigationMatrix[i, j, k].position = new Vector3(i * nodeSize, j * nodeSize, k * nodeSize);
                    navigationMatrix[i, j, k].index = new Vector3(i, j, k);

                    navigationMatrix[i, j, k].passable = true;

                    //ray cast to the neighbours of the node.
                    //if the ray cast collides with terrain, set passable to false
                    foreach (Vector3 dir in directions) //iterate through all directions
                    {
                        RaycastHit hit;

                        float rayLength = Mathf.Sqrt(Mathf.Pow(nodeSize, 2) + Mathf.Pow(nodeSize, 2) + (Mathf.Pow(nodeSize, 2)));

                        


                        if (Physics.Raycast(
                            navigationMatrix[i, j, k].position, 
                            dir, out hit, 
                            rayLength/2, 
                            layerMask))  //fire a ray in that direction, with a length of nodesize / 2
                        {
                            //Debug.DrawRay(navigationMatrix[i, j, k].position, dir * rayLength / 2, nodeDebugColourImpassable, 100000.0f);
                            navigationMatrix[i, j, k].passable = false; 
                        }
                        else
                        {
                            //Debug.DrawRay(navigationMatrix[i, j, k].position, dir * rayLength / 2, nodeDebugColourPassable, 100000.0f);
                        }
                    }


                    //if (Physics.CheckSphere(new Vector3(i * nodeSize, j * nodeSize, k * nodeSize), nodeSize/2, layerMask))
                    //{
                    //    navigationMatrix[i, j, k].passable = false;
                    //}
                    //else
                    //{
                    //    navigationMatrix[i, j, k].passable = true;
                    //}

                    navigationMatrix[i, j, k] = navigationMatrix[i, j, k].assignNeighbours(i, j, k, (int)Mathf.Floor(x / nodeSize), (int)Mathf.Floor(y / nodeSize), (int)Mathf.Floor(z / nodeSize));
                }
            }
        }
    }

    public List<Vector3> FindPath(Vector3 startPoint, Vector3 endPoint)
    {
        //get the start and end points via the parameters passed.
        gridNode startNode = navigationMatrix[(int)MathF.Round(startPoint.x / nodeSize), (int)MathF.Round(startPoint.y / nodeSize), (int)MathF.Round(startPoint.z / nodeSize)];
        gridNode endNode = navigationMatrix[(int)MathF.Round(endPoint.x / nodeSize), (int)MathF.Round(endPoint.y / nodeSize), (int)MathF.Round(endPoint.z / nodeSize)];

        //cleaning lists
        //openList.Clear();
        var sortedClosedList = new SortedSet<gridNode>(new NodeComparer());
        sortedClosedList.Clear();

        //Setting the g and h cost of the start node by getting the distance between the positions of the start and end nodes.
        startNode.gCost = 0;
        startNode.hCost = (int)Vector3.Distance(startNode.position, endNode.position);
        startNode.previousNodeIndex = startNode.index;

        var sortedOpenList = new SortedSet<gridNode>(new NodeComparer());  //using a sorted queue is more efficient, better time complexity (O(n))
        sortedOpenList.Add(startNode); //add the start node to the openlist

        while (sortedOpenList.Count > 0)  //while there are nodes in the open list
        {
            iterator++;
            //gridNode currentNode = navigationMatrix[(int)openList[0].x, (int)openList[0].y, (int)openList[0].z]; //current node is the first entry in the list (currently the only one, and the one it is at
            gridNode currentNode = sortedOpenList.Min;
            sortedOpenList.Remove(currentNode);

            //remove the current node from the open list and add it to the closed list, since it has now been visited
            sortedClosedList.Add(currentNode);

            if(currentNode.position == endNode.position)
            {
                return RemakePath(currentNode, startNode.index); //remake the path when you reach the next node                
            }

            foreach (Vector3 neighbourPos in currentNode.neighbour)
            {
                //gets the neighbour node
                if ((int)neighbourPos.x == -1)
                {
                    continue;
                }
                
                gridNode neighbourNode = navigationMatrix[(int)(neighbourPos.x), (int)(neighbourPos.y), (int)(neighbourPos.z)];

                if (!neighbourNode.passable || sortedClosedList.Contains(neighbourNode))
                {
                    continue;
                }

                //otherwise get the estimated gCost to reach the neighbour node from the start node
                int estimatedGCost = currentNode.gCost + (int)Vector3.Distance(currentNode.position, neighbourNode.position);
                
                //if the neighbour is not already in the open list, or if the estimated cost is lower than the current gCost
                if (!sortedOpenList.Contains(neighbourNode) || estimatedGCost < neighbourNode.gCost)
                {
                    neighbourNode.gCost = estimatedGCost; //update the gCost of the neighbour
                    neighbourNode.hCost = (int)Vector3.Distance(neighbourNode.position, endNode.position);//get the hcost of the new neighbour
                   
                    neighbourNode.previousNodeIndex = currentNode.index;//update the previous node position to that of the current one
                    
                    navigationMatrix[(int)neighbourNode.index.x, (int)neighbourNode.index.y, (int)neighbourNode.index.z] = neighbourNode;

                    if (!sortedOpenList.Contains(neighbourNode))    //if the node isn't already in the list, add it
                    {
                        sortedOpenList.Add(neighbourNode);    //add the neighbour node to the open list
                    }
                }
            }
        }
        return null;    //No Path found
    }

    private List<Vector3> RemakePath(gridNode currentNode, Vector3 originalNode)
    {
        List<Vector3> newPath = new List<Vector3>();    //make a new list for the new path
        List<gridNode> testing = new List<gridNode>();

        iterator2 = 0;
        while (currentNode.index != originalNode)    //iterate through the path from end to start (backwards)
        {

            iterator2++;
            newPath.Add(currentNode.position);  //add currentNode.position to the new path
            testing.Add(currentNode);
            currentNode = navigationMatrix[(int)(currentNode.previousNodeIndex.x), (int)(currentNode.previousNodeIndex.y), (int)(currentNode.previousNodeIndex.z)];   //move to the previous node

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
        if (navigationMatrix != null)
        {
            foreach (gridNode node in navigationMatrix)
            {
                if (node.passable)
                {
                    Gizmos.color = nodeDebugColourPassable;
                }
                else
                {
                    Gizmos.color = nodeDebugColourImpassable;
                }
                
                Gizmos.DrawSphere(node.position, 1);
            }
        }
        
    }

    
    //This entire class is heavily AI assisted.
    public class NodeComparer : IComparer<gridNode> //uses an IComparer (a built in c# thing), helps sort things in order
    {
        public int Compare(gridNode x, gridNode y)
        {
            int fCostComparison = x.GetFCost().CompareTo(y.GetFCost());     //compare x and y fcost, and check to see if one preceeds the other

            if (fCostComparison == 0)
            {
                // If fCost is the same, compare by gCost
                return x.gCost.CompareTo(y.gCost);
            }

            return fCostComparison; //return fcost
        }
    }
}

