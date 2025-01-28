using System;
using UnityEngine;

namespace Gravitas.Demo
{
    /// <summary>Implementation of a first person player controller that interacts with Gravitas systems.</summary>
    public class GravitasFirstPersonPlayerSubject : GravitasSubject
    {
        public Camera PlayerCamera => playerCamera;

        private const float MAX_GROUND_SPEED = 8f; // Maximum speed to clamp player movement to when player is landed

        public event Action<string> OnInteractionTargetEvent; // Event to communicate interaction target to UI

        [SerializeField] private Camera playerCamera; // The camera used by the player, typically a child of the player
        [SerializeField] private LayerMask interactableLayers = Physics.DefaultRaycastLayers;
        [SerializeField] private ParticleSystem playerParticleSystem; // Jetpack particle system to play on movement
        private Vector2 keyInput;
        private float
            angleX, // Stored camera pitch value
            verticalInput; // Stored vertical input from jumping or jetpack thrust
        [SerializeField] private float jetpackForce = 15f;
        [SerializeField] private float jumpForce = 7f;
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float turnSpeed = 5f;

        private bool interact;

        public bool playerOnWheel = false;

        /// <summary>
        /// Convenience method to instantly set player position, orientation, and stop all velocity.
        /// </summary>
        /// <param name="position">Position to set player's subject to</param>
        /// <param name="forward">Direction to make player's subject look at</param>
        public void SetPlayerSubjectPositionAndRotation(Vector3 position, Vector3 forward)
        {
            // Setting position and stopping velocity
            //gravitasBody.ProxyPosition = position;
            gravitasBody.Velocity = Vector3.zero;

            // Setting rotation and stopping angular velocity
            playerCamera.transform.localRotation = Quaternion.identity;
            //SetProxyLookRotation(forward);
            gravitasBody.AngularVelocity = Vector3.zero;
        }

        protected override void OnSubjectAwake()
        {
            base.OnSubjectAwake();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Input.ResetInputAxes();
        }

        protected override void OnSubjectUpdate()
        {
            base.OnSubjectUpdate();

            // Reload scene control
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
            {
                GravitasSceneManager.ReloadMainScene();

                return;
            }

            Transform t = gravitasBody.CurrentTransform; // Reference to either the player or the player's proxy transform

            // Movement input processing
            keyInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            
            // Player rotating
            Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            t.rotation *= Quaternion.AngleAxis(mouseInput.x * turnSpeed, Vector3.up);

            // Camera pitching
            angleX += -mouseInput.y * turnSpeed;

            if (gravitasBody.IsLanded)
            {
                angleX = Mathf.Clamp(angleX, -90f, 90f);
            }

            playerCamera.transform.localRotation = Quaternion.Euler(angleX, 0, 0);

            // Vertical input
            if (Input.GetKey(KeyCode.Space))
                verticalInput = 1; // Up
            else
                verticalInput = 0; // None

            // Interaction input
            if (!interact)
                interact = Input.GetKeyDown(KeyCode.E);
          
        }

        protected override void OnSubjectFixedUpdate()
        {
            base.OnSubjectFixedUpdate();

            Transform t = gravitasBody.CurrentTransform;

            bool isLanded = gravitasBody.IsLanded;
            Vector3 inputVelocity = GetInputVelocity();

            //sets the players velocity and adds a jump force
            if (isLanded)
            {
                gravitasBody.Velocity = inputVelocity.normalized * moveSpeed;
                gravitasBody.AddForce(new Vector3(0, inputVelocity.y, 0) * Time.deltaTime, ForceMode.VelocityChange);
            }

            //Controlls the velocity and force when in air
            if (!isLanded)
            {
                //clamps the velocity
                if (Mathf.Abs(gravitasBody.Velocity.x) > moveSpeed || Mathf.Abs(gravitasBody.Velocity.z) > moveSpeed)
                {
                    Vector3 normlaisedVel = gravitasBody.Velocity.normalized;
                    gravitasBody.Velocity = new Vector3(normlaisedVel.x * moveSpeed, gravitasBody.Velocity.y, normlaisedVel.z * moveSpeed);
                }

                //adds air controll
                gravitasBody.AddForce(new Vector3(inputVelocity.x, 0, inputVelocity.z) * 4f * Time.deltaTime, ForceMode.VelocityChange);
            }

            ProcessInteractionRaycast();
            interact = false;

            /// <summary>
            /// Method for processing the various possible inputs resulting from a player interaction input.
            /// </summary>
            void ProcessInteractionRaycast()
            {
                const float INTERACTION_DISTANCE = 2.75f;

                if // World raycasting
                (
                    Physics.Raycast
                    (
                        playerCamera.transform.position,
                        playerCamera.transform.forward,
                        out RaycastHit hitInfo,
                        INTERACTION_DISTANCE,
                        interactableLayers,
                        QueryTriggerInteraction.Ignore
                    )
                )
                {
                    // Interact with spaceship controls
                    if (hitInfo.collider.TryGetComponent(out GravitasSpaceshipControls spaceshipControls) && spaceshipControls.CanActivate)
                    {
                        if (interact)
                        {
                            #if GRAVITAS_LOGGING
                            if (GravitasDebugLogger.CanLog(GravitasDebugLoggingFlags.PlayerInteraction))
                                GravitasDebugLogger.Log($"Taking control of spaceship {spaceshipControls.SpaceshipName}");
                            #endif

                            spaceshipControls.InteractWithSpaceshipControls(this);
                            OnInteractionTargetEvent?.Invoke(string.Empty);
                        }
                        else
                        {
                            OnInteractionTargetEvent?.Invoke(spaceshipControls.SpaceshipName);
                        }
                    }
                    // Interact with field direction control
                    else if (hitInfo.collider.TryGetComponent(out GravitasFieldDirectionControl fieldDirectionControl))
                    {
                        if (interact)
                        {
                            #if GRAVITAS_LOGGING
                            if (GravitasDebugLogger.CanLog(GravitasDebugLoggingFlags.PlayerInteraction))
                                GravitasDebugLogger.Log($"Switching field direction to {fieldDirectionControl.DirectionName}");
                            #endif

                            fieldDirectionControl.SwitchGravity();
                        }
                        else
                        {
                            OnInteractionTargetEvent?.Invoke(fieldDirectionControl.DirectionName);
                        }
                    }
                    // Interact with spaceship reset button
                    else if (hitInfo.collider.TryGetComponent(out GravitasSpaceshipResetButton spaceshipResetButton))
                    {
                        if (interact)
                        {
                            #if GRAVITAS_LOGGING
                            if (GravitasDebugLogger.CanLog(GravitasDebugLoggingFlags.PlayerInteraction))
                                GravitasDebugLogger.Log("Resetting spaceship");
                            #endif

                            spaceshipResetButton.ResetSpaceship();
                        }
                        else
                        {
                            OnInteractionTargetEvent?.Invoke("Reset Button");
                        }
                    }
                }
                else
                {
                    OnInteractionTargetEvent?.Invoke(string.Empty);
                }
            }

            /// <summary>
            /// Local function for processing all movement inputs and returning the calculated movement velocity.
            /// </summary>
            /// <returns>Vector3 The calculated velocity</returns>
            Vector3 GetInputVelocity()
            {
                if(!playerOnWheel)
                {
                    Vector3 velocity = Vector3.zero;

                    // Left-Right movement
                    Vector3 right = t.right;
                    float xForce = moveSpeed;
                    Vector3 velocityx = keyInput.x * xForce * right;

                    // Up-Down movement
                    float yForce = jumpForce;
                    Vector3 velocityY = verticalInput * yForce * t.up;
                    Debug.Log(verticalInput);

                    // Forward-Back movement
                    Vector3 forward = t.forward;
                    float zForce = moveSpeed;
                    Vector3 velocityZ = keyInput.y * zForce * forward;

                    velocity = velocityx + velocityZ + velocityY;
                    return velocity;

                }
                else
                {
                    return Vector3.zero;
                }
            }
        }

        void OnApplicationFocus(bool focusStatus)
        {
            Input.ResetInputAxes();
        }
    }
}
