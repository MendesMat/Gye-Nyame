using GyeNyame.Player.Input;
using UnityEngine;

namespace GyeNyame.Player.Movement
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Rigidbody rb;
        [SerializeField] private PlayerInputHandler playerInput;

        [Header("Movement Properties")]
        [SerializeField] private float speed;
        [SerializeField] private float depthSpeedMultiplier;

        [Header("Jump Properties")]
        [SerializeField] private float gravity;
        [SerializeField] private float jumpForce;

        private float groundYPosition;
        private bool isGrounded;
        private float verticalVelocity;

        private void Awake() => groundYPosition = transform.position.y;
        private void OnEnable() => playerInput.OnJumpPressed += HandleJump;
        private void OnDisable() => playerInput.OnJumpPressed -= HandleJump;

        void FixedUpdate()
        {
            // Movement handling
            var moveInput = playerInput.MoveInput;
            Vector3 movement = new (moveInput.x, 0f, moveInput.y * depthSpeedMultiplier);
            var targetVelocity = movement * speed * Time.fixedDeltaTime;
            var targetPosition = rb.position + targetVelocity;

            // Jump and gravity handling
            if (!isGrounded) verticalVelocity -= gravity * Time.fixedDeltaTime;
            targetPosition.y += verticalVelocity * Time.fixedDeltaTime;

            if (!isGrounded && verticalVelocity <= 0f && targetPosition.y <= groundYPosition)
            {
                targetPosition.y = groundYPosition;
                verticalVelocity = 0f;
                isGrounded = true;
            }

            // Applying physics-based movement
            rb.MovePosition(targetPosition);
        }

        private void HandleJump()
        {
            if (isGrounded)
            {
                verticalVelocity = jumpForce;
                isGrounded = false;
            }
        }
    }
}
