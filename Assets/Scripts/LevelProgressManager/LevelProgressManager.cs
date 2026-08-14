using System;
using UnityEngine;

namespace PickAndMatch.Core
{
    // Lưu tiến trình người chơi: level nào đã mở khóa, mỗi level đạt bao nhiêu sao.
    // Dùng PlayerPrefs nên dữ liệu vẫn còn sau khi tắt game.
    // Đặt object này ở scene đầu tiên (hoặc scene "Bootstrap") và không bị destroy khi đổi scene.
    public class LevelProgressManager : MonoBehaviour
    {
        public static LevelProgressManager Instance { get; private set; }

        private const string UnlockedKeyPrefix = "Level_Unlocked_";
        private const string StarsKeyPrefix = "Level_Stars_";

        [Header("Cấu hình")]
        [Tooltip("Level đầu tiên luôn mở sẵn, không cần hoàn thành level nào trước đó.")]
        [SerializeField] private int firstLevel = 1;

        public event Action OnProgressChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            UnlockLevel(firstLevel);
        }

        public bool IsLevelUnlocked(int level)
        {
            if (level <= firstLevel)
                return true;

            return PlayerPrefs.GetInt(UnlockedKeyPrefix + level, 0) == 1;
        }

        public int GetStars(int level)
        {
            return PlayerPrefs.GetInt(StarsKeyPrefix + level, 0);
        }

        public void UnlockLevel(int level)
        {
            if (IsLevelUnlocked(level))
                return;

            PlayerPrefs.SetInt(UnlockedKeyPrefix + level, 1);
            PlayerPrefs.Save();

            OnProgressChanged?.Invoke();
        }

      public void CompleteLevel(int level, int stars)
{
    int bestStars = Mathf.Max(stars, GetStars(level));
    PlayerPrefs.SetInt(StarsKeyPrefix + level, bestStars);
    PlayerPrefs.Save();


    UnlockLevel(level + 1);

    OnProgressChanged?.Invoke();
}

        public void ResetProgress(int maxLevel)
        {
            for (int level = firstLevel; level <= maxLevel; level++)
            {
                PlayerPrefs.DeleteKey(UnlockedKeyPrefix + level);
                PlayerPrefs.DeleteKey(StarsKeyPrefix + level);
            }

            PlayerPrefs.Save();
            OnProgressChanged?.Invoke();
        }
    }
}