using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor.PackageManager;

public class SceneAdditive : MonoBehaviour
{
    [SerializeField] private string shipInteriorSceneString = "PhysicsScene";
    [SerializeField] private Scene shipInteriorScene;
    [SerializeField] private Transform mainSceneShipTransform;
    [SerializeField] private PhysicsScene shipInteriorPhysicsScene;

    private void Awake()
    {
        if (!string.IsNullOrEmpty(shipInteriorSceneString))
        {
            // Subscribe to the sceneLoaded event
            SceneManager.sceneLoaded += OnSceneLoaded;

            // Load the scene additively
            SceneManager.LoadScene(shipInteriorSceneString, LoadSceneMode.Additive);
        }

        mainSceneShipTransform = GameObject.FindWithTag("Ship").transform;

        Physics.simulationMode = SimulationMode.Script;

    }

    // This method will be called when the additive scene is fully loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == shipInteriorSceneString)
        {
            shipInteriorScene = scene;
            shipInteriorPhysicsScene = shipInteriorScene.GetPhysicsScene();

            ParentSceneToShip();

            // Unsubscribe to prevent repeated calls
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void ParentSceneToShip()
    {
        // Check if the scene has root game objects
        GameObject[] rootGameObjects = shipInteriorScene.GetRootGameObjects();

        if (rootGameObjects.Length > 0)
        {
            GameObject physicsSceneRoot = rootGameObjects[0];
            physicsSceneRoot.transform.SetParent(mainSceneShipTransform);
        }
        else
        {
            Debug.LogError("No root game objects found in the ship interior scene.");
        }
    }

    private void FixedUpdate()
    {
        if (shipInteriorPhysicsScene.IsValid())
        {
            // Manually simulate the physics for the interior physics scene
            shipInteriorPhysicsScene.Simulate(Time.fixedDeltaTime);
        }
        else
        {
            Debug.Log("Error with validating ship interior physics");
        }
    }

    private void LateUpdate()
    {
        if (shipInteriorPhysicsScene.IsValid())
        {
            // Match rotation of the interior scene to the ship
            //Physics.autoSyncTransforms = false;
            //transform.rotation = mainSceneShipTransform.rotation;

            // Optional: Match position if needed
            //transform.position = mainSceneShipTransform.position;
            Vector3 Gravy = mainSceneShipTransform.up * -9.8f;
            Physics.gravity = Gravy;
        }
        else
        {
            Debug.Log("Error with validating ship interior physics");
        }
    }
}

