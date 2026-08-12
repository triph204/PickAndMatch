using UnityEngine;

namespace PickAndMatch.Core
{
    public class ReadyState : IGameState
    {
        private readonly GameManager gameManager;

        public ReadyState(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public void Enter()
        {
            Debug.Log("Game State: READY");
        }

        public void Exit()
        {
        }

        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                gameManager.StartGame();
            }
        }
    }
}