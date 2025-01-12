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

    public struct gridNode
    {
        public Vector3 position;

        public int traversalCost;

        //absolute chonker of a struct constructor
        public gridNode(Vector3 Position, int traversalCost)
        {
            this.position = Position;
            this.traversalCost = traversalCost;

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
        PopulateWorld(1000, 500, 1000, 10);

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
    private void PopulateWorld(float x, float y, float z, float nodeSpacing)
    {
        Debug.Log("Populating World");
        navigationMatrix = new gridNode
            [(int)Mathf.Floor(x / nodeSpacing),    //gets the number of nodes for the worlds width, x
            (int)Mathf.Floor(y / nodeSpacing),    //get the number of nodes for the worlds height, y
            (int)Mathf.Floor(z / nodeSpacing)];    //get the number of nodes for the worlds depth, z

        //iterate through the three dimensional array, going through width, then height, then depth (x, y ,z)
        for(int i = 0; i < navigationMatrix.GetLength(0); i++)            
        {
            for (int j = 0; j < navigationMatrix.GetLength(1); j++)
            {
                for (int k = 0; k < navigationMatrix.GetLength(2); k++)
                {
                    navigationMatrix[i, j, k].position = new Vector3(i * nodeSpacing, j * nodeSpacing, k * nodeSpacing);
                    navigationMatrix[i, j, k].GetAdjacentNodes(i, j, k, 1000, 500, 1000);    //calls the function with the current indexes as the parameters
                }
            }
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
