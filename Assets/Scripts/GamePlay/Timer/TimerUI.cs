using UnityEngine;
using UnityEngine.UI;
using PickAndMatch.Gameplay.Timer;

namespace PickAndMatch.UI
{
    public class TimerUI : MonoBehaviour
    {
        [SerializeField] private GameTimer gameTimer;
        [SerializeField] private Slider timerSlider;

        private void Start()
        {
            if (gameTimer == null)
            {
                return;
            }

            if (timerSlider == null)
            {
                return;
            }

            // Hiển thị ban đầu
            timerSlider.value = 1f;

            // Nhận sự kiện timer
            gameTimer.OnTimeChanged += UpdateSlider;

            // TEST: tự động chạy timer
            gameTimer.StartTimer();
        }

        private void UpdateSlider(float currentTime)
        {
            float normalized =
                gameTimer.NormalizedTime;

            timerSlider.value = normalized;

            Debug.Log(
                $"Timer: {currentTime:F1} | Slider: {normalized:F2}");
        }

        private void OnDestroy()
        {
            if (gameTimer != null)
            {
                gameTimer.OnTimeChanged -= UpdateSlider;
            }
        }
    }
}