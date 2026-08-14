using System.Collections;
using UnityEngine;
using PickAndMatch.Core;

public class UIManager : MonoBehaviour
{
    public GameObject[] allPanels; // Kéo TẤT CẢ panel vào đây

    [Header("Open Animation")]
    [SerializeField] private float popDuration = 0.3f;
    [SerializeField] private float overshootScale = 1.15f;

    [Header("Close Animation")]
    [SerializeField] private float closeDuration = 0.2f;

    // Coroutine đang chạy cho từng panel, để tránh mở/đóng chồng nhau gây giật.
    private readonly System.Collections.Generic.Dictionary<GameObject, Coroutine> activeRoutines =
        new System.Collections.Generic.Dictionary<GameObject, Coroutine>();

    public void ShowPanel(GameObject panelToShow)
    {
        foreach (var panel in allPanels)
        {
            if (panel == panelToShow)
                continue;

            if (panel.activeSelf)
            {
                PlayClose(panel);
            }
        }

        if (panelToShow != null)
        {
            PlayOpen(panelToShow);

            // Panel đang mở -> chặn click xuyên xuống thẻ bên dưới.
            GameManager.Instance?.SetBoardInputEnabled(false);
        }
    }

    public void HideAllPanels()
    {
        foreach (var panel in allPanels)
        {
            if (panel.activeSelf)
            {
                PlayClose(panel);
            }
        }

        // Không còn panel nào mở -> cho click thẻ lại bình thường.
        GameManager.Instance?.SetBoardInputEnabled(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // ----- Mở panel: 0 -> phình to hơn 1 chút -> về đúng 1 -----
    private void PlayOpen(GameObject panel)
    {
        StopExistingRoutine(panel);

        Coroutine routine = StartCoroutine(OpenRoutine(panel));
        activeRoutines[panel] = routine;
    }

    private IEnumerator OpenRoutine(GameObject panel)
    {
        panel.SetActive(true);
        panel.transform.localScale = Vector3.zero;

        yield return ScaleTo(panel.transform, overshootScale, popDuration * 0.6f);
        yield return ScaleTo(panel.transform, 1f, popDuration * 0.4f);

        activeRoutines.Remove(panel);
    }

    // ----- Đóng panel: co nhỏ dần về 0 rồi mới ẩn hẳn -----
    private void PlayClose(GameObject panel)
    {
        StopExistingRoutine(panel);

        Coroutine routine = StartCoroutine(CloseRoutine(panel));
        activeRoutines[panel] = routine;
    }

    private IEnumerator CloseRoutine(GameObject panel)
    {
        yield return ScaleTo(panel.transform, 0f, closeDuration);

        panel.SetActive(false);
        panel.transform.localScale = Vector3.one;

        activeRoutines.Remove(panel);
    }

    private void StopExistingRoutine(GameObject panel)
    {
        if (activeRoutines.TryGetValue(panel, out Coroutine routine) && routine != null)
        {
            StopCoroutine(routine);
            activeRoutines.Remove(panel);
        }
    }

    private IEnumerator ScaleTo(Transform target, float toScale, float duration)
    {
        float fromScale = target.localScale.x;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            float scale = Mathf.Lerp(fromScale, toScale, t);
            target.localScale = Vector3.one * scale;

            yield return null;
        }

        target.localScale = Vector3.one * toScale;
    }
}