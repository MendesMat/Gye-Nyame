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
        public UnityEvent onOpenHitbox;
        public UnityEvent onCloseHitbox;

        public void OpenCancelWindow() => onOpenCancelWindow?.Invoke();
        public void CloseCancelWindow() => onCloseCancelWindow?.Invoke();
        public void FinishAttack() => onFinishAttack?.Invoke();
        public void OpenHitbox() => onOpenHitbox?.Invoke();
        public void CloseHitbox() => onCloseHitbox?.Invoke();
    }
}
