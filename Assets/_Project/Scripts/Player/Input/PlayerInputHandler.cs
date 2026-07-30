using UnityEngine;
using UnityEngine.InputSystem;
using GyeNyame.Core.Events;
using GyeNyame.Core.Contracts.Messages;

namespace GyeNyame.Player.Input
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private PlayerInputActions _inputActions;

        private void Awake() => _inputActions = new PlayerInputActions();

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerDiedMessage>(OnPlayerDied);
            _inputActions.Player.Enable();

            _inputActions.Player.Move.performed += OnMovePerformed;
            _inputActions.Player.Move.canceled += OnMoveCanceled;
            _inputActions.Player.Jump.performed += OnJumpPerformed;
            _inputActions.Player.Dash.performed += OnDashPerformed;
            _inputActions.Player.AttackLight.performed += OnAttackLightPerformed;
            _inputActions.Player.AttackHeavy.performed += OnAttackHeavyPerformed;
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerDiedMessage>(OnPlayerDied);
            _inputActions.Player.Move.performed -= OnMovePerformed;
            _inputActions.Player.Move.canceled -= OnMoveCanceled;
            _inputActions.Player.Jump.performed -= OnJumpPerformed;
            _inputActions.Player.Dash.performed -= OnDashPerformed;
            _inputActions.Player.AttackLight.performed -= OnAttackLightPerformed;
            _inputActions.Player.AttackHeavy.performed -= OnAttackHeavyPerformed;

            _inputActions.Disable();
        }

        // Movement
        private void OnMovePerformed(InputAction.CallbackContext ctx)
            => EventBus.Publish(new PlayerMoveMessage(ctx.ReadValue<Vector2>()));

        private void OnMoveCanceled(InputAction.CallbackContext ctx)
            => EventBus.Publish(new PlayerMoveMessage(Vector2.zero));

        // Actions
        private void OnJumpPerformed(InputAction.CallbackContext ctx)
            => EventBus.Publish(new PlayerJumpMessage());

        private void OnDashPerformed(InputAction.CallbackContext ctx)
            => EventBus.Publish(new PlayerDashMessage());

        private void OnAttackLightPerformed(InputAction.CallbackContext ctx)
            => EventBus.Publish(new PlayerAttackLightMessage());

        private void OnAttackHeavyPerformed(InputAction.CallbackContext ctx)
            => EventBus.Publish(new PlayerAttackHeavyMessage());

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

        private void OnPlayerDied(PlayerDiedMessage message)
        {
            SwitchToUIMode();
        }
    }
}
