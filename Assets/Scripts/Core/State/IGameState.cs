namespace PickAndMatch.Core
{
    public interface IGameState
    {
        void Enter();
        void Exit();
        void Update();
    }
}