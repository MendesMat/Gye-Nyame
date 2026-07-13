using System;
using GyeNyame.Core.EventBus;

namespace GyeNyame.Core.InputBuffer
{
    public struct BufferedCommand
    {
        public Type MessageType { get; }
        public float Timestamp { get; }
        public float ExpirationTime { get; }

        public BufferedCommand(Type messageType, float timestamp, float expirationTime)
        {
            MessageType = messageType;
            Timestamp = timestamp;
            ExpirationTime = expirationTime;
        }

        public bool IsValid(float currentTime)
        {
            return currentTime <= ExpirationTime;
        }
    }
}
