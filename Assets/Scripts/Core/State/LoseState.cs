using UnityEngine;

namespace PickAndMatch.Core
{
    public class LoseState : IGameState
    {
        public void Enter()
        {
            Debug.Log("GAME LOSE");
        }

        public void Exit()
        {
        }

        public void Update()
        {
        }
    }
}