using GyeNyame.Core.Events;

namespace GyeNyame.Core.Contracts.Messages
{
    public readonly struct PlayerAttackLightMessage : IMessage { } 
    public readonly struct PlayerAttackHeavyMessage : IMessage { }
    public readonly struct EndCombatMessage : IMessage { }
}
