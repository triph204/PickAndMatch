using UnityEngine;
using UnityEngine.UI;
using PickAndMatch.Core;

namespace PickAndMatch.UI
{
    public class LevelButtonUI : MonoBehaviour
    {
        [Header("Level")]
        [Tooltip("Phải trùng LevelNumber trong BoardData của level này.")]
        [SerializeField] private int levelNumber;
        [Tooltip("Tên scene load khi bấm nút.")]
        [SerializeField] private string sceneName;

        [Header("References")]
        [SerializeField] private Button button;
        [Tooltip("Icon ổ khóa, hiện khi level CHƯA mở.")]
        [SerializeField] private GameObject lockIcon;
        [Tooltip("Nội dung chỉ hiện khi level ĐÃ mở (VD số thứ tự level).")]
        [SerializeField] private GameObject contentWhenUnlocked;

        [Header("Stars")]
        [Tooltip("3 GameObject ngôi sao, đúng thứ tự sao 1 - 2 - 3.")]
        [SerializeField] private GameObject[] starIcons;

        private void Reset()
        {
            button = GetComponent<Button>();
        }

        private void Start()
        {
            Refresh();

            if (LevelProgressManager.Instance != null)
            {
                LevelProgressManager.Instance.OnProgressChanged += Refresh;
            }
        }

        private void OnDisable()
        {
            if (LevelProgressManager.Instance != null)
                LevelProgressManager.Instance.OnProgressChanged -= Refresh;
        }

        public void Refresh()
        {
            bool unlocked =
                LevelProgressManager.Instance == null ||
                LevelProgressManager.Instance.IsLevelUnlocked(levelNumber);

            if (lockIcon != null)
                lockIcon.SetActive(!unlocked);

            if (contentWhenUnlocked != null)
                contentWhenUnlocked.SetActive(unlocked);

            if (button != null)
                button.interactable = unlocked;

            int stars =
                LevelProgressManager.Instance != null
                    ? LevelProgressManager.Instance.GetStars(levelNumber)
                    : 0;

            RefreshStars(stars, unlocked);
        }

        private void RefreshStars(int stars, bool unlocked)
        {
            if (starIcons == null)
                return;

            for (int i = 0; i < starIcons.Length; i++)
            {
                if (starIcons[i] == null)
                    continue;

                starIcons[i].SetActive(unlocked && i < stars);
            }
        }

        public void OnClickPlay()
        {
            if (LevelProgressManager.Instance != null &&
                !LevelProgressManager.Instance.IsLevelUnlocked(levelNumber))
            {
                return;
            }

            if (string.IsNullOrEmpty(sceneName))
                return;

            if (SceneLoad.Instance != null)
                SceneLoad.Instance.LoadScene(sceneName);
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}