using GyeNyame.Core.EventBus;

namespace GyeNyame.Core.Contracts.Messages
{
    public readonly struct PlayerAttackLightMessage : IMessage { } 
    public readonly struct PlayerAttackHeavyMessage : IMessage { }
    public readonly struct EndCombatMessage : IMessage { }
}
