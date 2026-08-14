using System.Collections;
using UnityEngine;

namespace PickAndMatch.UI
{
    public class GameResultUI : MonoBehaviour
    {
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject losePanel;

        [Header("Stars")]
        [Tooltip("3 GameObject ngôi sao, kéo đúng thứ tự sao 1 - sao 2 - sao 3.")]
        [SerializeField] private GameObject[] starIcons;

        [Header("Panel Animation")]
        [SerializeField] private float popDuration = 0.3f;
        [SerializeField] private float overshootScale = 1.15f;

        [Header("Star Animation")]
        [SerializeField] private float starPopDuration = 0.2f;
        [SerializeField] private float starOvershootScale = 1.3f;
        [Tooltip("Thời gian chờ giữa mỗi sao nảy ra.")]
        [SerializeField] private float starDelay = 0.15f;

        private Coroutine panelPopRoutine;
        private Coroutine starsRoutine;

        private void Awake()
        {
            if (winPanel != null)
                winPanel.SetActive(false);

            if (losePanel != null)
                losePanel.SetActive(false);
        }

        public void ShowWin(int stars)
        {
            Debug.Log($"SHOW WIN PANEL - {stars} stars");

            if (winPanel == null)
            {
                Debug.LogError("WinPanel chưa được gán!");
                return;
            }

            ResetStars();

            if (losePanel != null)
                losePanel.SetActive(false);

            PlayPanelPop(winPanel);

            if (starsRoutine != null)
            {
                StopCoroutine(starsRoutine);
            }

            starsRoutine = StartCoroutine(AnimateStarsRoutine(stars));
        }

        private void ResetStars()
        {
            if (starIcons == null)
                return;

            foreach (GameObject star in starIcons)
            {
                if (star != null)
                {
                    star.SetActive(false);
                    star.transform.localScale = Vector3.zero;
                }
            }
        }

        private IEnumerator AnimateStarsRoutine(int starsCount)
        {
            if (starIcons == null)
                yield break;

            // Đợi panel nảy ra được nửa chừng thì mới bắt đầu tới sao.
            yield return new WaitForSecondsRealtime(popDuration * 0.5f);

            for (int i = 0; i < starIcons.Length; i++)
            {
                if (i >= starsCount || starIcons[i] == null)
                    continue;

                yield return StartCoroutine(
                    PopRoutine(starIcons[i], starPopDuration, starOvershootScale));

                yield return new WaitForSecondsRealtime(starDelay);
            }

            starsRoutine = null;
        }

        public void ShowLose()
        {
            Debug.Log("SHOW LOSE PANEL");

            if (losePanel == null)
            {
                Debug.LogError("LosePanel chưa được gán!");
                return;
            }

            if (winPanel != null)
                winPanel.SetActive(false);

            PlayPanelPop(losePanel);
        }

        private void PlayPanelPop(GameObject panel)
        {
            if (panelPopRoutine != null)
            {
                StopCoroutine(panelPopRoutine);
            }

            panelPopRoutine = StartCoroutine(
                PopRoutine(panel, popDuration, overshootScale));
        }

        // Animation "nảy" dùng chung: 0 -> phình to hơn 1 chút -> về đúng 1.
        // Dùng chung cho cả panel lẫn từng ngôi sao (duration/overshoot khác nhau).
        private IEnumerator PopRoutine(GameObject target, float duration, float overshoot)
        {
            target.SetActive(true);
            target.transform.localScale = Vector3.zero;

            float phase1 = duration * 0.6f;
            float phase2 = duration * 0.4f;

            float elapsed = 0f;

            while (elapsed < phase1)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / phase1);

                float scale = Mathf.Lerp(0f, overshoot, t);
                target.transform.localScale = Vector3.one * scale;

                yield return null;
            }

            elapsed = 0f;

            while (elapsed < phase2)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / phase2);

                float scale = Mathf.Lerp(overshoot, 1f, t);
                target.transform.localScale = Vector3.one * scale;

                yield return null;
            }

            target.transform.localScale = Vector3.one;
        }
    }
}