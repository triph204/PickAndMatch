using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PickAndMatch.Core
{
    public class SceneLoad : MonoBehaviour
    {
        public static SceneLoad Instance
        {
            get;
            private set;
        }

        [Header("Fade (tuỳ chọn)")]
        [Tooltip("CanvasGroup phủ toàn màn hình (màu đen), để trống nếu không cần hiệu ứng fade.")]
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private float fadeDuration = 0.3f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

        }

        public void RestartScene()
        {
            LoadScene(SceneManager.GetActiveScene().name);
        }

     
        public void LoadScene(string sceneName)
        {
            // Time.timeScale có thể đang = 0 (lúc Pause) -> trả về 1 trước khi qua scene khác.
            Time.timeScale = 1f;

            if (fadeCanvasGroup != null)
            {
                StartCoroutine(FadeAndLoad(sceneName, -1));
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        }


        private IEnumerator FadeAndLoad(string sceneName, int buildIndex)
        {
            yield return Fade(0f, 1f);

            if (sceneName != null)
            {
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                SceneManager.LoadScene(buildIndex);
            }

            yield return Fade(1f, 0f);
        }

        private IEnumerator Fade(float from, float to)
        {
            fadeCanvasGroup.blocksRaycasts = true;

            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / fadeDuration);

                fadeCanvasGroup.alpha = Mathf.Lerp(from, to, t);

                yield return null;
            }

            fadeCanvasGroup.alpha = to;
            fadeCanvasGroup.blocksRaycasts = to > 0f;
        }
    }
}