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
}
