public interface IStateMachine<T> where T : StateBase
{
    #region Properties
    public StateMachine<T> StateMachine { get; }

    public T CurrentState { get; }

    public T PrevState { get; }
    #endregion

    #region Methods
    public void ChangeState(T nextState);
    #endregion
}