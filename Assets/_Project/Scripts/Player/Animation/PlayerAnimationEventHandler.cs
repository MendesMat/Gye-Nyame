using UnityEngine;
using GyeNyame.Core.EventBus;
using GyeNyame.Core.Contracts.Messages;

namespace GyeNyame.Player.Animation
{
    public class PlayerAnimationEventHandler : MonoBehaviour
    {
        public void OpenCancelWindow() => EventBus.Publish(new AnimationCancelWindowMessage(true));
        public void CloseCancelWindow() => EventBus.Publish(new AnimationCancelWindowMessage(false));
        public void FinishAttack() => EventBus.Publish(new AnimationFinishAttackMessage());
    }
}
