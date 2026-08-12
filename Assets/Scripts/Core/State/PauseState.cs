using UnityEngine;

namespace PickAndMatch.Core
{
    public class PauseState : IGameState
    {
        public void Enter()
        {
            Debug.Log("Game State: PAUSED");

            Time.timeScale = 0f;
        }

        public void Exit()
        {
            Time.timeScale = 1f;
        }

        public void Update()
        {
        }
    }
}