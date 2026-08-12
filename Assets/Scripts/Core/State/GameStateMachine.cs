namespace PickAndMatch.Core
{
    public class GameStateMachine
    {
        private IGameState currentState;

        public IGameState CurrentState =>
            currentState;

        public void ChangeState(IGameState newState)
        {
            if (newState == null)
                return;

            currentState?.Exit();

            currentState = newState;

            currentState.Enter();
        }

        public void Update()
        {
            currentState?.Update();
        }
    }
}