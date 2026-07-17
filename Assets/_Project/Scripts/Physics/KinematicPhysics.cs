using UnityEngine;
using GyeNyame.Core.Contracts.Interfaces;

namespace GyeNyame.Physics
{
    [RequireComponent(typeof(Rigidbody))]
    public class KinematicPhysics : MonoBehaviour, IKinematicPhysics
    {
        [Header("Components")]
        [SerializeField] private Rigidbody _rigidbody;

        [Header("Ground Check")]
        [SerializeField] private LayerMask groundLayerMask;
        [SerializeField] private float groundCheckRadius = 0.3f;
        [SerializeField] private Vector3 groundCheckOffset = Vector3.up * 0.1f;

        [Header("Movement Collsion")]
        [SerializeField] private float skinWidth = 0.01f;

        #region IKinematicPhysics Implementation
        public bool CheckGround(Vector3 currentPosition, float fallDistance, out float allowedFallDistance)
        {
            var castOrigin = currentPosition + transform.rotation * groundCheckOffset;
            
            bool isHit = UnityEngine.Physics.SphereCast(
                castOrigin, 
                groundCheckRadius, 
                Vector3.down, 
                out RaycastHit hitInfo, 
                fallDistance, 
                groundLayerMask, 
                QueryTriggerInteraction.Ignore
            );

            if (isHit)
            {
                allowedFallDistance = Mathf.Max(0f, hitInfo.distance - skinWidth);
                return true;
            } 
            
            allowedFallDistance = fallDistance;
            return false;
        }

        public Vector3 CalculateAllowedMovement(Vector3 currentPosition, Vector3 intendedMovement)
        {
            float distance = intendedMovement.magnitude;
            
            if (distance <= 0f)
                return Vector3.zero;

            Vector3 direction = intendedMovement / distance;

            bool isHit = _rigidbody.SweepTest(
                direction, 
                out RaycastHit hitInfo, 
                distance + skinWidth, 
                QueryTriggerInteraction.Ignore
            );

            if (isHit)
            {
                float allowedDistance = Mathf.Max(0f, hitInfo.distance - skinWidth);
                return direction * allowedDistance;
            }

            return intendedMovement;
        }
        #endregion

        #region Editor / Gizmos
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            var castOrigin = transform.position + transform.rotation * groundCheckOffset;
            Gizmos.DrawWireSphere(castOrigin, groundCheckRadius);
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(castOrigin, castOrigin + Vector3.down * 0.5f);
        }
        #endregion
    }
}
