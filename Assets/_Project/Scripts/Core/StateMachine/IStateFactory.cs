namespace GyeNyame.Core.StateMachine
{
    public interface IStateFactory
    {
        BaseState Create(IStateMachine stateMachine);
    }
}
