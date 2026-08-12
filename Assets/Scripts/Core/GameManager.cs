using UnityEngine;
using PickAndMatch.Gameplay.Board;

namespace PickAndMatch.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance
        {
            get;
            private set;
        }

        [Header("References")]
        [SerializeField]
        private BoardManager boardManager;

        private GameStateMachine stateMachine;

        private void Awake()
        {
            if (Instance != null &&
                Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            stateMachine =
                new GameStateMachine();
        }

        private void Start()
        {
            StartGame();
        }

        private void Update()
        {
            stateMachine?.Update();
        }

        public void StartGame()
        {
            boardManager.GenerateBoard();

            stateMachine.ChangeState(
                new PlayingState(this));
        }

        public void PauseGame()
        {
            stateMachine.ChangeState(
                new PauseState());
        }

        public void ResumeGame()
        {
            stateMachine.ChangeState(
                new PlayingState(this));
        }

        public void WinGame()
        {
            stateMachine.ChangeState(
                new WinState());
        }

        public void LoseGame()
        {
            stateMachine.ChangeState(
                new LoseState());
        }
    }
}