using UnityEngine;

namespace PickAndMatch.Core
{
    public class WinState : IGameState
    {
        public void Enter()
        {
            Debug.Log("GAME WIN");
        }

        public void Exit()
        {
        }

        public void Update()
        {
        }
    }
}