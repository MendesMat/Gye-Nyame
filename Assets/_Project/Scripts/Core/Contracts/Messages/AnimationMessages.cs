using GyeNyame.Core.Events;

namespace GyeNyame.Core.Contracts.Messages
{
    public readonly struct AnimationCancelWindowMessage : IMessage 
    {
        public bool IsOpen { get; }

        public AnimationCancelWindowMessage(bool isOpen)
        {
            IsOpen = isOpen;
        }
    }
    
    public readonly struct AnimationFinishAttackMessage : IMessage { }

    public readonly struct AnimationFinishDeathMessage : IMessage
    {
        public UnityEngine.GameObject Target { get; }
        public AnimationFinishDeathMessage(UnityEngine.GameObject target) => Target = target;
    }
}
