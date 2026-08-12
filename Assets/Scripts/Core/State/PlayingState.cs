using UnityEngine;

namespace PickAndMatch.Core
{
    public class PlayingState : IGameState
    {
        private readonly GameManager gameManager;

        public PlayingState(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public void Enter()
        {
            Debug.Log("Game State: PLAYING");
        }

        public void Exit()
        {
        }

        public void Update()
        {
        }
    }
}