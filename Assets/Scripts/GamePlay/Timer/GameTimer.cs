using System;
using UnityEngine;

namespace PickAndMatch.Gameplay.Timer
{
    public class GameTimer : MonoBehaviour
    {
        [SerializeField] private float defaultTime = 60f;

        private float currentTime;
        private float maxTime;
        private bool isRunning;

        public float CurrentTime => currentTime;

        public float NormalizedTime
        {
            get
            {
                if (maxTime <= 0f)
                    return 0f;

                return currentTime / maxTime;
            }
        }

        public event Action<float> OnTimeChanged;
        public event Action OnTimeOut;

        private void Update()
        {
            if (!isRunning)
                return;

            currentTime -= Time.deltaTime;

            if (currentTime <= 0f)
            {
                currentTime = 0f;
                isRunning = false;

                // HẾT GIỜ → DỪNG TIẾNG TICK
                StopTickSound();

                OnTimeChanged?.Invoke(currentTime);
                OnTimeOut?.Invoke();

                return;
            }

            OnTimeChanged?.Invoke(currentTime);
        }

        public void StartTimer()
        {
            StartTimer(defaultTime);
        }

        public void StartTimer(float duration)
        {
            maxTime = duration;
            currentTime = duration;
            isRunning = true;

            // BẮT ĐẦU TIẾNG TICK
            StartTickSound();

            OnTimeChanged?.Invoke(currentTime);

            Debug.Log($"Timer Started: {duration} seconds");
        }

        public void StopTimer()
        {
            isRunning = false;

            StopTickSound();
        }

        public void PauseTimer()
        {
            isRunning = false;

            StopTickSound();
        }

        public void ResumeTimer()
        {
            if (currentTime > 0f)
            {
                isRunning = true;

                StartTickSound();
            }
        }

        private void StartTickSound()
        {
            if (AudioManager.Instance == null)
                return;

            if (AudioManager.Instance.tick == null)
                return;

            // Cho tick loop
            AudioManager.Instance.tick.loop = true;

            // Tránh Play lại nếu đang chạy
            if (!AudioManager.Instance.tick.isPlaying)
            {
                AudioManager.Instance.PlaySound(AudioManager.Instance.tick);
            }
        }

        private void StopTickSound()
        {
            if (AudioManager.Instance == null)
                return;

            if (AudioManager.Instance.tick == null)
                return;

            AudioManager.Instance.tick.Stop();
        }
    }
}