using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipVelocity : MonoBehaviour
{
    [SerializeField] float maxVelocity = 10f;
    [SerializeField] float speed = 3f;
    [SerializeField] float rotSpeed = 3f;
    [SerializeField] bool playerControllingShip = false;
    [SerializeField] float currentVelocity;

    [SerializeField] List<GameObject> playersList = new List<GameObject>();

    private float verticalInput;
    private float horizontalInput;

    [SerializeField] Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (playerControllingShip)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");

            AddVelocityToShip();
            RotateShip();
        }
        //currentVelocity = rb.velocity.z;
    }

    private void AddVelocityToShip()
    {
        rb.velocity = verticalInput * this.transform.forward * Time.deltaTime * speed + rb.velocity;
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);

        //TODO Convert CURRENT velocity into the same amount of velocity however facing forwards over time.
        //gather total magnitude of velocity 
        //add percentage of that velocity to the ships forwards, same time remove same percentage from all other velcoity directions.
        //might work maybe

        //foreach(GameObject Player in  playersList)
        //{
        //    Player.transform.GetComponent<Rigidbody>().velocity = rb.velocity;
        //}
    }
    private void RotateShip()
    {
        float direction = horizontalInput * Time.deltaTime;

        //this.transform.rotation = Quaternion.Euler(0f,direction,0f);

        this.transform.rotation = Quaternion.Euler(new Vector3(0f, direction * rotSpeed, 0f) + this.transform.rotation.eulerAngles);

    }


    public bool AddPlayerToList(GameObject Player)
    {
        if (!playersList.Contains(Player))
        {
            playersList.Add(Player);
            return true;
        }
        return false;
    }
    public bool RemovePlayerFromList(GameObject Player)
    {
        if (playersList.Contains(Player))
        {
            playersList.Remove(Player);
            return true;
        }
        return false;
    }

}
