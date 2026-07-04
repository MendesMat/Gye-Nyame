using UnityEngine;
using GyeNyame.Core.EventBus;

namespace GyeNyame.Player.Contracts.Messages
{
    public readonly struct PlayerMoveMessage : IMessage
    {
        public Vector2 MoveInput { get; }

        public PlayerMoveMessage(Vector2 moveInput) => MoveInput = moveInput;
    }
}
