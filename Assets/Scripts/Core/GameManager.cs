using UnityEngine;
using PickAndMatch.Gameplay.Board;
using PickAndMatch.Gameplay.Score;
using PickAndMatch.Gameplay.Timer;
using PickAndMatch.UI;

namespace PickAndMatch.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private GameTimer gameTimer;
        [SerializeField] private GameResultUI gameResultUI;
        [SerializeField] private UIManager uiManager;

        private GameStateMachine stateMachine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            stateMachine = new GameStateMachine();

            if (gameTimer != null)
            {
                gameTimer.OnTimeOut += HandleTimeOut;
            }
        }

        private void Start()
        {
            ChangeToReadyState();
            StartGame();
        }

        private void Update()
        {
            stateMachine?.Update();
        }

        public void StartGame()
        {
            if (boardManager == null)
            {
                Debug.LogError("BoardManager is missing!");
                return;
            }

            // Đề phòng còn panel nào đang mở từ trước (VD restart khi đang Pause).
            uiManager?.HideAllPanels();

            boardManager.GenerateBoard();
            boardManager.SetInputEnabled(true);

            if (gameTimer != null)
            {
                gameTimer.StartTimer(boardManager.TimeLimit);
            }

            stateMachine.ChangeState(
                new PlayingState(this));
        }

        public void PauseGame()
        {
            if (gameTimer != null)
            {
                gameTimer.PauseTimer();
            }

            stateMachine.ChangeState(
                new PauseState());
        }

        public void ResumeGame()
        {
            if (gameTimer != null)
            {
                gameTimer.ResumeTimer();
            }

            // Đóng panel (VD Pause Panel) và mở lại click board — chỉ cần bấm nút Resume là đủ.
            uiManager?.HideAllPanels();

            stateMachine.ChangeState(
                new PlayingState(this));
        }

        public void WinGame()
        {
            int stars = 1;

            if (gameTimer != null)
            {
                stars = StarCalculator.Calculate(gameTimer.NormalizedTime);
                gameTimer.StopTimer();
            }

            if (boardManager != null)
            {
                boardManager.SetInputEnabled(false);

                if (LevelProgressManager.Instance != null)
                {

                    LevelProgressManager.Instance.CompleteLevel(
                        boardManager.LevelNumber,
                        stars);
                }
            }

            if (gameResultUI != null)
            {
                gameResultUI.ShowWin(stars);
            }

            stateMachine.ChangeState(
                new WinState());
        }

        public void LoseGame()
        {
            if (gameTimer != null)
            {
                gameTimer.StopTimer();
            }

            if (boardManager != null)
            {
                boardManager.SetInputEnabled(false);
            }

            if (gameResultUI != null)
            {
                gameResultUI.ShowLose();
            }

            stateMachine.ChangeState(
                new LoseState());
        }

        // Gọi bởi UIManager mỗi khi mở/đóng panel bất kỳ (settings, shop, pause menu...)
        // để board không nhận click xuyên qua panel.
        public void SetBoardInputEnabled(bool enabled)
        {
            if (boardManager != null)
            {
                boardManager.SetInputEnabled(enabled);
            }
        }

        private void HandleTimeOut()
        {
            Debug.Log("TIME OUT!");

            LoseGame();
        }

        private void ChangeToReadyState()
        {
            stateMachine.ChangeState(
                new ReadyState(this));
        }

        private void OnDestroy()
        {
            if (gameTimer != null)
            {
                gameTimer.OnTimeOut -= HandleTimeOut;
            }
        }
    }
}