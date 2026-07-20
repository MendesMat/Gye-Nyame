using UnityEngine;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Core.StateMachine;

namespace GyeNyame.Agents.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class BaseEntityMovement : MonoBehaviour, IEntityLocomotion
    {
        [SerializeField] protected StateMachine stateMachine;
        [SerializeField] protected Rigidbody rigidBody;
        [SerializeField] protected float speed = 5f;
        [SerializeField] protected float depthSpeedMultiplier = 0.5f;
        [SerializeField] protected float gravity = 25f;
        [SerializeField] protected float knockbackDeceleration = 15f;

        protected IKinematicPhysics kinematicPhysics;
        protected bool isGrounded;
        protected float verticalVelocity;
        protected Vector2 currentMoveInput;
        protected Vector2 facingDirection = Vector2.right;
        protected float facingDirectionX = 1f;
        protected bool isFacingDirectionLocked;
        protected Vector3 externalForceVelocity;

        public bool HasMoveInput => currentMoveInput.sqrMagnitude > 0.01f;
        public bool IsGrounded => isGrounded;
        public float VerticalVelocity => verticalVelocity;
        public float FacingDirectionX => facingDirectionX;

        protected virtual void Awake()
        {
            kinematicPhysics = GetComponent<IKinematicPhysics>();
        }

        public void SetFacingDirectionLock(bool isLocked)
        {
            isFacingDirectionLocked = isLocked;
            
            if (isLocked) return;
            
            UpdateFacingDirection();
        }

        public virtual void DisablePhysics()
        {
            if (rigidBody != null)
            {
                if (!rigidBody.isKinematic)
                {
                    rigidBody.linearVelocity = Vector3.zero;
                    rigidBody.isKinematic = true;
                }
            }
        }

        protected virtual void UpdateFacingDirection()
        {
            if (currentMoveInput.x != 0f)
                facingDirectionX = Mathf.Sign(currentMoveInput.x);

            if (currentMoveInput != Vector2.zero)
                facingDirection = currentMoveInput.normalized;
        }

        public virtual void UpdateMovement(float speedMultiplier, bool lockDepth = false)
        {
            var currentPosition = rigidBody.position;
            var depthInput = lockDepth ? 0f : currentMoveInput.y;
            var directionMovement = new Vector3(currentMoveInput.x, 0f, depthInput * depthSpeedMultiplier) * speedMultiplier;
            var intendedMovement = directionMovement * speed * Time.fixedDeltaTime;

            ApplyExternalForceDeceleration();
            intendedMovement += externalForceVelocity * Time.fixedDeltaTime;

            var allowedMovement = kinematicPhysics.CalculateAllowedMovement(currentPosition, intendedMovement);
            var targetPosition = currentPosition + allowedMovement;

            targetPosition = ApplyVerticalMovement(targetPosition);
            rigidBody.MovePosition(targetPosition);
        }

        protected virtual Vector3 ApplyVerticalMovement(Vector3 currentPosition)
        {
            if (!isGrounded) 
                verticalVelocity -= gravity * Time.fixedDeltaTime;
            
            float intendedFall = verticalVelocity * Time.fixedDeltaTime;

            if (intendedFall < 0f)
            {
                float fallDistance = Mathf.Abs(intendedFall);
                if (kinematicPhysics.CheckGround(currentPosition, fallDistance, out float allowedFall))
                {
                    verticalVelocity = 0f;
                    isGrounded = true;
                    OnLanded();
                    currentPosition.y -= allowedFall;
                    return currentPosition;
                }
            }
            
            if (intendedFall >= 0f && isGrounded)
            {
                if (!kinematicPhysics.CheckGround(currentPosition, 0.05f, out _))
                    isGrounded = false;
            }

            currentPosition.y += intendedFall;
            return currentPosition;
        }

        protected virtual void OnLanded() { }

        public virtual void ApplyExternalForce(Vector3 direction, float force, float vertVelocity)
        {
            verticalVelocity = vertVelocity;
            isGrounded = false;
            
            externalForceVelocity = direction * force;
        }

        protected virtual void ApplyExternalForceDeceleration()
        {
            if (externalForceVelocity.sqrMagnitude <= 0.01f)
            {
                externalForceVelocity = Vector3.zero;
                return;
            }

            float currentDeceleration = isGrounded ? knockbackDeceleration : (knockbackDeceleration * 0.1f);
            externalForceVelocity = Vector3.Lerp(externalForceVelocity, Vector3.zero, Time.fixedDeltaTime * currentDeceleration);
        }
    }
}
