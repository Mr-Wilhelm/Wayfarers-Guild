using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace Gravitas
{
    /// <summary>Implementation of a physics body that can be proxied.</summary>
    [AddComponentMenu("Gravitas/Gravitas Body")]
    [DisallowMultipleComponent]
    public class GravitasBody : MonoBehaviour, IGravitasBody
    {
        public Rigidbody CurrentRigidbody => IsProxied ? currentProxy.ProxyRigidbody : gravitasBodyRigidbody;
        public Rigidbody Rigidbody => gravitasBodyRigidbody;
        public Transform CurrentTransform => IsProxied ? currentProxy.transform : transform;
        public Vector3 AngularVelocity
        {
            get
            {
                Rigidbody rb = CurrentRigidbody;
                if (rb)
                    return rb.angularVelocity;

                return Vector3.zero;
            }
            set
            {
                Rigidbody rb = CurrentRigidbody;
                if (rb)
                    rb.angularVelocity = value;
            }
        }
        public Vector3 ProxyPosition => IsProxied ? currentProxy.transform.localPosition : Vector3.zero;
        public Vector3 Velocity
        {
            get
            {
                Rigidbody rb = CurrentRigidbody;
                if (rb)
                    return rb.velocity;

                return Vector3.zero;
            }
            set
            {
                Rigidbody rb = CurrentRigidbody;
                if (rb)
                    rb.velocity = value;
            }
        }
        public bool IsLanded
        {
            get => IsProxied && isLanded;
            set => isLanded = value;
        }
        public bool IsProxied => (bool)currentProxy;

        protected GravitasSubjectProxy currentProxy;

        private RigidbodySettings previousRigidbodySettings;
        [SerializeField] private List<Collider> bodyColliders;
        [SerializeField] private Rigidbody gravitasBodyRigidbody;
        private bool isLanded;

        private GameObject teleportAnchor;
        private bool blockGravitas = false;

        private GameObject lockAnchor;
        private bool lockedInPlace = false;

        /// <summary>
        /// Adds a force either to this subject's rigidbody, or the proxy's rigidbody if it exists.
        /// </summary>
        /// <param name="force">The force to add</param>
        /// <param name="forceMode">The type of force behaviour to use</param>
        public virtual void AddForce(Vector3 force, ForceMode forceMode)
        {
            Rigidbody rb = CurrentRigidbody;
            if (rb)
                rb.AddForce(force, forceMode);
        }

        /// <summary>
        /// Adds a torque force to this subject's rigidbody, or the proxy's rigidbody if it exists.
        /// </summary>
        /// <param name="torque">The torque force to add</param>
        /// <param name="forceMode">The type of force behaviour to use</param>
        public virtual void AddTorque(Vector3 torque, ForceMode forceMode)
        {
            Rigidbody rb = CurrentRigidbody;
            if (rb)
                rb.AddTorque(torque, forceMode);
        }

        public virtual void AddRelativeTorque(Vector3 torque, ForceMode forceMode)
        {
            Rigidbody rb = CurrentRigidbody;
            if (rb)
                rb.AddRelativeTorque(torque, forceMode);
        }

        public virtual void MoveRotation(Quaternion rot)
        {
            Rigidbody rb = CurrentRigidbody;
            if (rb)
                rb.MoveRotation(rot);
        }

        public virtual void MovePosition(Vector3 pos)
        {
            Rigidbody rb = CurrentRigidbody;
            if (rb)
                rb.MovePosition(pos);
        }

        public virtual void unblockGravitas()
        {
            blockGravitas = false;
        }

        public virtual void teleportPlayer(GameObject teleportObject)
        {
            teleportAnchor = teleportObject;
            blockGravitas = true;
        }

        public virtual void lockPosition(GameObject lockObject)
        {
            lockAnchor = lockObject;
            lockedInPlace = true;
        }

        public virtual void unLockPosition()
        {
            lockedInPlace = false;
        }

        public virtual void delayedUnLockPosition(float time)
        {
            Invoke(nameof(unLockPosition), time);
        }

        public virtual void freezeConstraints()
        {
            Rigidbody rb = CurrentRigidbody;
            if (rb)
                rb.constraints = RigidbodyConstraints.FreezePosition;
        }

        public virtual void unfreezeConstraints()
        {
            Rigidbody rb = CurrentRigidbody;
            if (rb)
                rb.constraints = RigidbodyConstraints.None;
        }

        public virtual Quaternion rotation()
        {
            Rigidbody rb = CurrentRigidbody;
            if (rb)
                return rb.rotation;
            else
            {
                return Quaternion.identity;
            }
        }

        /// <summary>Automatically finds and assigns colliders that belong to this body.</summary>
        public void AutoFindBodyColliders()
        {
            bodyColliders ??= new List<Collider>();

            bodyColliders.Clear();

            foreach (Collider col in GetComponentsInChildren<Collider>(false))
            {
                if (col && !col.isTrigger)
                    bodyColliders.Add(col);
            }
        }

        public virtual void SetProxy(GravitasSubjectProxy proxy)
        {
            if (proxy)
            {
                previousRigidbodySettings = new RigidbodySettings(gravitasBodyRigidbody);
                currentProxy = proxy;

                gravitasBodyRigidbody.isKinematic = true;
            }
        }

        /// <summary>
        /// Called when removing this subject from a field, performs final disconnections between this subject and its proxy.
        /// </summary>
        public virtual void DestroyProxy()
        {
            isLanded = false;

            if (IsProxied)
            {
                previousRigidbodySettings.AssignTo(ref gravitasBodyRigidbody);

                Destroy(currentProxy.gameObject);

                currentProxy = null;
            }
        }

        public virtual void Orient(Vector3 up, float orientSpeed)
        {
            Transform currentTransform = CurrentTransform;
            float angle = Vector3.Angle(transform.up, up);
            if (angle > 0.05f)
            {
                Quaternion
                    currentRotation = currentTransform.rotation,
                    rot = Quaternion.FromToRotation(currentTransform.up, up) * currentRotation;
                currentTransform.rotation = Quaternion.RotateTowards(currentRotation, rot, orientSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// Updates this subject's position to match its proxy's, using the current field transform.
        /// </summary>
        public virtual void UpdatePosition(Transform fieldTransform)
        {
            //Debug.Log("Weiner Normal");
            if (blockGravitas)
            {

                if (IsProxied && fieldTransform)
                {
                    if (teleportAnchor != null)
                    {
                        
                        Transform proxyTransform = currentProxy.transform;
                        transform.SetPositionAndRotation
                        (
                            teleportAnchor.transform.position,
                            Quaternion.LookRotation
                            (
                                fieldTransform.TransformDirection(proxyTransform.forward),
                                fieldTransform.TransformDirection(proxyTransform.up)
                            )
                        );
                        currentProxy.transform.position = teleportAnchor.transform.position;
                    }
                    
                }

                

                blockGravitas = false;
            }
            else if (lockedInPlace)
            {
                if (IsProxied && fieldTransform)
                {
                    Transform proxyTransform = currentProxy.transform;
                    transform.SetPositionAndRotation
                    (
                        lockAnchor.transform.position,
                        Quaternion.LookRotation
                        (
                            fieldTransform.TransformDirection(proxyTransform.forward),
                            fieldTransform.TransformDirection(proxyTransform.up)
                        )
                    );
                }

                //currentProxy.transform.position = lockAnchor.transform.position;
                isLanded = true;
            }
            else
            {
                if (IsProxied && fieldTransform)
                {
                    Transform proxyTransform = currentProxy.transform;
                    transform.SetPositionAndRotation
                    (
                        fieldTransform.TransformPointUnscaled(proxyTransform.localPosition),
                        Quaternion.LookRotation
                        (
                            fieldTransform.TransformDirection(proxyTransform.forward),
                            fieldTransform.TransformDirection(proxyTransform.up)
                        )
                    );
                }
            }
            
            
        }

        public virtual Collider[] GetBodyColliders()
        {
            return bodyColliders?.ToArray();
        }

        #if UNITY_EDITOR
        public int CheckBodyErrors(out string errorMessage)
        {
            // 0 = No error
            // 1 = Warning
            // 2 = Error

            errorMessage = null;

            if (!gravitasBodyRigidbody)
            {
                errorMessage = "No rigidbody assigned for body!";

                return 2;
            }
            else if (!Application.isPlaying && gravitasBodyRigidbody.isKinematic)
            {
                errorMessage = $"Rigidbody '{gravitasBodyRigidbody.name}' is kinematic, and will not move!";

                return 1;
            }
            else if (!Application.isPlaying && gravitasBodyRigidbody.useGravity)
            {
                errorMessage = $"Rigidbody '{gravitasBodyRigidbody.name}' is set to use global gravity. This setting will be overridden.";

                return 1;
            }


            if (!HasAnyColliders(bodyColliders))
            {
                errorMessage = "No Colliders are assigned to be used with this body!";

                return 1;
            }

            return 0;

            static bool HasAnyColliders(in List<Collider> colliders)
            {
                if (colliders == null || colliders.Count == 0) { return false; }

                int length = colliders.Count;
                for (int i = 0; i < length; i++)
                {
                    if (colliders[i])
                        return true;
                }

                return false;
            }
        }
        #endif

        protected void Awake()
        {
            //TODO: error if no rigidbody

            // Warnings against required fields not being assigned
            if (bodyColliders == null || bodyColliders.Count == 0)
            {
                Debug.LogWarning($@"Gravitas: No Colliders assigned to subject ""{gameObject.name}"", attempting to find");
                AutoFindBodyColliders();
            }
        }



        private void Update()
        {
            if (lockedInPlace)
            {
                IsLanded = true;
            }
        }

    }

    

}
