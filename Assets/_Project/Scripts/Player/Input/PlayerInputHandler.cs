using UnityEngine;
using UnityEngine.InputSystem;
using GyeNyame.Core.EventBus;
using GyeNyame.Player.Contracts.Messages;

namespace GyeNyame.Player.Input
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private PlayerInputActions _inputActions;

        private void Awake() => _inputActions = new PlayerInputActions();

        private void OnEnable()
        {
            _inputActions.Player.Enable();

            _inputActions.Player.Move.performed += OnMovePerformed;
            _inputActions.Player.Move.canceled += OnMoveCanceled;

            _inputActions.Player.Jump.performed += OnJumpPerformed;
        }

        private void OnDisable()
        {
            _inputActions.Player.Move.performed -= OnMovePerformed;
            _inputActions.Player.Move.canceled -= OnMoveCanceled;

            _inputActions.Player.Jump.performed -= OnJumpPerformed;

            _inputActions.Player.Disable();
        }

        // Movement
        private void OnMovePerformed(InputAction.CallbackContext ctx)
            => EventBus.Publish(new PlayerMoveMessage(ctx.ReadValue<Vector2>()));

        private void OnMoveCanceled(InputAction.CallbackContext ctx)
            => EventBus.Publish(new PlayerMoveMessage(Vector2.zero));

        // Jump
        private void OnJumpPerformed(InputAction.CallbackContext ctx)
            => EventBus.Publish(new PlayerJumpMessage());

        // Switch Input Mode
        public void SwitchToPlayerMode()
        {
            _inputActions.UI.Disable();
            _inputActions.Player.Enable();
        }

        public void SwitchToUIMode()
        {
            _inputActions.Player.Disable();
            _inputActions.UI.Enable();
        }
    }
}
