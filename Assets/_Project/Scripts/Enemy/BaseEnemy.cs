using UnityEngine;
using GyeNyame.Core.Contracts.Data;
using GyeNyame.Core.Contracts.Interfaces;
using GyeNyame.Core.Events;
using GyeNyame.Core.Contracts.Messages;

namespace GyeNyame.Enemy
{
    public abstract class BaseEnemy : MonoBehaviour
    {
        [Header("Components")]
        protected IKinematicPhysics kinematicPhysics;

        [Header("Physics Settings")]
        [SerializeField] protected float gravity = 25f;
        [SerializeField] protected float knockbackDeceleration = 5f;

        protected Vector3 currentKnockbackVelocity;
        protected float currentVerticalVelocity;
        protected bool isGrounded;

        protected virtual void Awake()
        {
            
            kinematicPhysics = GetComponent<IKinematicPhysics>();
            if (kinematicPhysics == null)
            {
                Debug.LogError("IKinematicPhysics not found on Enemy object!", this);
                return;
            }
        }

        protected virtual void FixedUpdate()
        {
            ApplyGravity();
            ApplyKnockbackDeceleration();
            CalculateMovement();
        }

        private void ApplyGravity()
        {
            if (isGrounded) return;
            
            currentVerticalVelocity -= gravity * Time.fixedDeltaTime;
        }

        private void ApplyKnockbackDeceleration()
        {
            if (currentKnockbackVelocity.sqrMagnitude <= 0.01f)
            {
                currentKnockbackVelocity = Vector3.zero;
                return;
            }
            
            currentKnockbackVelocity = Vector3.Lerp(currentKnockbackVelocity, Vector3.zero, Time.fixedDeltaTime * knockbackDeceleration);
        }

        private void CalculateMovement()
        {
            if (kinematicPhysics == null) return;

            Vector3 targetPosition = CalculateHorizontalPosition();
            targetPosition = CalculateVerticalPosition(targetPosition);

            transform.position = targetPosition;
        }

        private Vector3 CalculateHorizontalPosition()
        {
            Vector3 intendedMovement = currentKnockbackVelocity * Time.fixedDeltaTime;
            Vector3 allowedMovement = kinematicPhysics.CalculateAllowedMovement(transform.position, intendedMovement);
            
            return transform.position + allowedMovement;
        }

        private Vector3 CalculateVerticalPosition(Vector3 currentPosition)
        {
            float intendedFall = currentVerticalVelocity * Time.fixedDeltaTime;
            
            if (intendedFall >= 0f)
            {
                isGrounded = false;
                currentPosition.y += intendedFall;
                
                return currentPosition;
            }

            return ProcessFalling(currentPosition, intendedFall);
        }

        private Vector3 ProcessFalling(Vector3 currentPosition, float intendedFall)
        {
            bool hitGround = kinematicPhysics.CheckGround(currentPosition, Mathf.Abs(intendedFall), out float allowedFall);
            
            if (hitGround) return HandleGroundCollision(currentPosition, allowedFall);

            isGrounded = false;
            currentPosition.y += intendedFall;
            
            return currentPosition;
        }

        private Vector3 HandleGroundCollision(Vector3 currentPosition, float allowedFall)
        {
            currentVerticalVelocity = 0f;
            isGrounded = true;
            currentPosition.y -= allowedFall;
            
            return currentPosition;
        }

        protected virtual void OnEnable()
        {
            EventBus.Subscribe<EntityDamagedMessage>(OnEntityDamaged);
        }

        protected virtual void OnDisable()
        {
            EventBus.Unsubscribe<EntityDamagedMessage>(OnEntityDamaged);
        }

        private void OnEntityDamaged(EntityDamagedMessage message)
        {
            if (message.Target == gameObject) ApplyKnockback(message.Damage);
        }

        private void ApplyKnockback(DamageData data)
        {
            Vector3 knockbackDirection = (transform.position - data.SourcePosition).normalized;
            knockbackDirection.y = 0;

            currentKnockbackVelocity = knockbackDirection * data.KnockbackForce;
            currentVerticalVelocity = data.KnockupForce;
            isGrounded = false;
        }
    }
}
