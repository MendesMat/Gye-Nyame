using System;
using UnityEngine;

namespace GyeNyame.Player.Input
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private PlayerInputActions inputActions;
        public Vector2 MoveInput { get; private set; }

        public event Action OnJumpPressed;

        private void Awake() => inputActions = new();

        void OnEnable()
        {
            inputActions.Player.Enable();

            inputActions.Player.Move.performed += OnMovePerformed;
            inputActions.Player.Move.canceled += OnMoveCanceled;
            inputActions.Player.Jump.performed += OnJumpPerformed;
        }

        private void OnDisable()
        {
            inputActions.Player.Move.performed -= OnMovePerformed;
            inputActions.Player.Move.canceled -= OnMoveCanceled;
            inputActions.Player.Jump.performed -= OnJumpPerformed;

            inputActions.Player.Disable();
        }

        private void OnMovePerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => MoveInput = ctx.ReadValue<Vector2>();
        private void OnMoveCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => MoveInput = Vector2.zero;
        private void OnJumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx) => OnJumpPressed?.Invoke();

        public void SwitchToPlayerMode()
        {
            inputActions.UI.Disable();
            inputActions.Player.Enable();
        }

        public void SwitchToUIMode()
        {
            inputActions.Player.Disable();
            inputActions.UI.Enable();
        }
    }
}
