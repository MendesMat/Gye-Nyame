using UnityEngine;
using GyeNyame.Core.Events;

namespace GyeNyame.Core.Contracts.Messages
{
    public readonly struct PlayerMoveMessage : IMessage
    {
        public Vector2 MoveInput { get; }

        public PlayerMoveMessage(Vector2 moveInput)
        {
            MoveInput = moveInput;
        }
    }

    public readonly struct PlayerJumpMessage : IMessage { }

    public readonly struct PlayerDashMessage : IMessage { }
}
