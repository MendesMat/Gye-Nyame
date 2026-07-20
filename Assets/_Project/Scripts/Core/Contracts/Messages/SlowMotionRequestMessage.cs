using GyeNyame.Core.Events;

namespace GyeNyame.Core.Contracts.Messages
{
    public struct SlowMotionRequestMessage : IMessage
    {
        public float TimeScale { get; }
        public float Duration { get; }

        public SlowMotionRequestMessage(float timeScale, float duration)
        {
            TimeScale = timeScale;
            Duration = duration;
        }
    }
}
