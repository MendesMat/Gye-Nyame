using UnityEngine;

namespace GyeNyame.Core.StateMachine
{
    [RequireComponent(typeof(StateMachine))]
    public class StateMachineDebugger : MonoBehaviour
    {
        [SerializeField] private bool showLogs = true;

        [SerializeField] private StateMachine _stateMachine;

        private void OnEnable()
        {
            if (_stateMachine == null) return;
            _stateMachine.OnStateChanged += HandleStateChanged;
        }

        private void OnDisable()
        {
            if (_stateMachine == null) return;
            _stateMachine.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(BaseState previousState, BaseState newState)
        {
            if (!showLogs) return;

            string previousStateName = previousState != null ? previousState.StateName : "None";
            string newStateName = newState != null ? newState.StateName : "None";

            Debug.Log($"[StateMachine - <b><color=cyan>{gameObject.name}</color></b>] Mudou de <color=orange>{previousStateName}</color> para <color=green>{newStateName}</color>.");
        }
    }
}
