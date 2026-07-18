using UnityEngine;
using UnityEngine.Events;

namespace GyeNyame.Combat.Animation
{
    public class CombatAnimationEventHandler : MonoBehaviour
    {
        [Header("Animation Events")]
        public UnityEvent onOpenCancelWindow;
        public UnityEvent onCloseCancelWindow;
        public UnityEvent onFinishAttack;

        [Header("Hitboxes")]
        [SerializeField] private Components.HitboxComponent[] hitboxes;

        public void OpenCancelWindow() => onOpenCancelWindow?.Invoke();
        public void CloseCancelWindow() => onCloseCancelWindow?.Invoke();
        public void FinishAttack() => onFinishAttack?.Invoke();

        public void EnableHitbox(int index)
        {
            if (index < 0 || index >= hitboxes.Length) return;
            hitboxes[index].EnableHitbox();
        }

        public void DisableHitbox(int index)
        {
            if (index < 0 || index >= hitboxes.Length) return;
            hitboxes[index].DisableHitbox();
        }
    }
}
