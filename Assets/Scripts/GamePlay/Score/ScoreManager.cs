using System;
using UnityEngine;

namespace PickAndMatch.Gameplay.Score
{
    public class ScoreManager : MonoBehaviour
    {
        private int score;

        public int Score => score;

        public event Action<int> OnScoreChanged;

        public void AddScore(int amount)
        {
            score += amount;

            OnScoreChanged?.Invoke(score);
        }

        public void ResetScore()
        {
            score = 0;

            OnScoreChanged?.Invoke(score);
        }
    }
}
