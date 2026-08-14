using TMPro;
using UnityEngine;
using PickAndMatch.Gameplay.Score;

namespace PickAndMatch.UI
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private TMP_Text scoreText;

        private void Start()
        {
            if (scoreManager == null)
            {
                return;
            }

            UpdateScoreText(scoreManager.Score);

            scoreManager.OnScoreChanged += UpdateScoreText;
        }

        private void UpdateScoreText(int newScore)
        {
            if (scoreText == null)
            {
                return;
            }

            scoreText.text = newScore.ToString();
        }

        private void OnDestroy()
        {
            if (scoreManager != null)
            {
                scoreManager.OnScoreChanged -= UpdateScoreText;
            }
        }
    }
}