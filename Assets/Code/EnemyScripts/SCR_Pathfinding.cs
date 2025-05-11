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
    public GridNode[,,] navigationMatrix;

    int xLength;
    int yLength;
    int zLength;

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

    [SerializeField]
    private GameObject airship;


    public class GridNode
    {
        public Vector3 position;
        public int gCost, hCost;
        public Vector3 previousNodeIndex;
        public Vector3 index;
        public bool passable;

        public int GetFCost()
        {
            return gCost + hCost;
        }



        public Vector3[] GetNeighbours()
        {
            var list = new List<Vector3>();
            for (float x = -1; x <= 1; x++)
                for (int y = -1; y <= 1; y++)
                    for (int z = -1; z <= 1; z++)
                        if (x != 0 || y != 0 || z != 0)
                            list.Add(new Vector3(x, y, z) + index);
            return list.ToArray();
        }

    }

    public bool IsWithinBounds(Vector3 index, int x, int y, int z)
    {
        return index.x >= 0 && index.x < x &&
               index.y >= 0 && index.y < y &&
               index.z >= 0 && index.z < z;
    }



    private void Awake()
    {
        PopulateWorld(endPos.x, endPos.y, endPos.z);
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
        xLength = Mathf.FloorToInt(x / nodeSize);
        yLength = Mathf.FloorToInt(y / nodeSize);
        zLength = Mathf.FloorToInt(z / nodeSize);

        navigationMatrix = new GridNode
            [xLength,    //gets the number of nodes for the worlds width, x
            yLength,    //get the number of nodes for the worlds height, y
            zLength];    //get the number of nodes for the worlds depth, z

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
                    navigationMatrix[i, j, k] = new GridNode
                    {
                        position = new Vector3(i * nodeSize, j * nodeSize, k * nodeSize),
                        index = new Vector3(i, j, k),
                        passable = !Physics.CheckBox(new Vector3(i * nodeSize, j * nodeSize, k * nodeSize), Vector3.one * (nodeSize / 2f), Quaternion.identity, layerMask),
                        gCost = 0,
                        hCost = 0,
                        previousNodeIndex = Vector3.zero
                    };
                }
            }
        }
    }

    public List<Vector3> FindPath(Vector3 startPoint, Vector3 endPoint)
    {

        //get the start and end points via the parameters passed.
        GridNode startNode = navigationMatrix[(int)MathF.Round(startPoint.x / nodeSize), (int)MathF.Round(startPoint.y / nodeSize), (int)MathF.Round(startPoint.z / nodeSize)];
        GridNode endNode = navigationMatrix[(int)MathF.Round(endPoint.x / nodeSize), (int)MathF.Round(endPoint.y / nodeSize), (int)MathF.Round(endPoint.z / nodeSize)];

        //cleaning lists
        //openList.Clear();
        var sortedClosedList = new SortedSet<GridNode>(new NodeComparer());
        sortedClosedList.Clear();

        //Setting the g and h cost of the start node by getting the distance between the positions of the start and end nodes.
        startNode.gCost = 0;
        startNode.hCost = (int)Vector3.Distance(startNode.position, endNode.position);
        startNode.previousNodeIndex = startNode.index;

        var sortedOpenList = new SortedSet<GridNode>(new NodeComparer());  //using a sorted queue is more efficient, better time complexity (O(n))
        sortedOpenList.Add(startNode); //add the start node to the openlist

        while (sortedOpenList.Count > 0)  //while there are nodes in the open list
        {
            //gridNode currentNode = navigationMatrix[(int)openList[0].x, (int)openList[0].y, (int)openList[0].z]; //current node is the first entry in the list (currently the only one, and the one it is at
            GridNode currentNode = sortedOpenList.Min;
            sortedOpenList.Remove(currentNode);

            //remove the current node from the open list and add it to the closed list, since it has now been visited
            sortedClosedList.Add(currentNode);

            if (currentNode.position == endNode.position)
            {
                return RemakePath(currentNode, startNode.index); //remake the path when you reach the next node                
            }

            foreach (Vector3 neighbourPos in currentNode.GetNeighbours())
            {
                //gets the neighbour node
                if (!IsWithinBounds(neighbourPos, xLength, yLength, zLength))
                {
                    continue;
                }

                GridNode neighbourNode = navigationMatrix[(int)(neighbourPos.x), (int)(neighbourPos.y), (int)(neighbourPos.z)];

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
            iterator++;
            if (iterator >= 500000)
            {
                Debug.Log("Break at 1st while loop");
                break;
            }
        }
        return null;    //No Path found
    }

    private List<Vector3> RemakePath(GridNode currentNode, Vector3 originalNode)
    {
        List<Vector3> newPath = new List<Vector3>();    //make a new list for the new path
        List<GridNode> testing = new List<GridNode>();

        iterator2 = 0;
        while (currentNode.index != originalNode)    //iterate through the path from end to start (backwards)
        {

            iterator2++;
            newPath.Add(currentNode.position);  //add currentNode.position to the new path
            testing.Add(currentNode);
            currentNode = navigationMatrix[(int)(currentNode.previousNodeIndex.x), (int)(currentNode.previousNodeIndex.y), (int)(currentNode.previousNodeIndex.z)];   //move to the previous node

            if (iterator2 >= 1000)
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

    //void OnDrawGizmos()
    //{
    //    if (navigationMatrix != null)
    //    {
    //        foreach (gridNode node in navigationMatrix)
    //        {

    //            if (Vector3.Distance(airship.transform.position, node.position) < 500)
    //            {
    //                if (node.passable)
    //                {
    //                    Gizmos.color = nodeDebugColourPassable;
    //                }
    //                else
    //                {
    //                    Gizmos.color = nodeDebugColourImpassable;
    //                }

    //                Gizmos.DrawSphere(node.position, 1);
    //            }

    //        }
    //    }

    //}


    //This entire class is heavily AI assisted.
    public class NodeComparer : IComparer<GridNode> //uses an IComparer (a built in c# thing), helps sort things in order
    {
        public int Compare(GridNode x, GridNode y)
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

