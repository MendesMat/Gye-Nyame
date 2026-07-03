using GyeNyame.Core.StateMachine;

namespace GyeNyame.Core.EventBus.Messages.StateMachine
{
    public class StateChangedMessage : IMessage
    {
        public BaseState Previous { get; }
        public BaseState Next { get; }

        public StateChangedMessage(BaseState previous, BaseState next)
        {
            Previous = previous;
            Next = next;
        }
    }
}
