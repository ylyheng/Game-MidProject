using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EndCreditsController : MonoBehaviour
{
    [Header("reference")]
    [SerializeField] AutoReplayAudio soundManager;

    [Header("UI")]
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform creditsContent;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float scrollDuration = 30f;

    [Tooltip("If true, credits begin scrolling at the same time as the fade-in. " +
             "If false, content stays parked at the start Y while fading in, then scrolls.")]
    [SerializeField] private bool scrollDuringFade = false;

    private Coroutine creditsRoutine;

    // Horizontal position captured from the inspector before the coroutine runs.
    // Y is intentionally animated by the scroll; X is preserved.
    private float inspectorX;

    private void Awake()
    {
        // Ensure the credits are hidden by default so nothing is visible
        // during the rest of the game.
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        if (creditsPanel != null && creditsPanel.activeSelf)
            creditsPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameCompleted += StartCredits;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameCompleted -= StartCredits;
    }

    private void StartCredits()
    {
        if(soundManager)soundManager.Play("ending");
        
        if (creditsRoutine != null) return;
        creditsRoutine = StartCoroutine(PlayCredits());
    }

    private IEnumerator PlayCredits()
    {
        if (creditsPanel == null || canvasGroup == null || creditsContent == null)
        {
            Debug.LogError("EndCreditsController: missing references.");
            creditsRoutine = null;
            yield break;
        }

        // ---------------------------------------------------------------
        // 1) SETUP — do everything BEFORE making anything visible so the
        //    first rendered frame is already correct.
        // ---------------------------------------------------------------

        // Hide immediately.
        canvasGroup.alpha = 0f;

        // Enable the panel so layout can be measured (invisible because
        // alpha is 0). This does NOT reset the anchoredPosition you set
        // in the inspector unless a layout group is actively driving it.
        creditsPanel.SetActive(true);

        // Force a layout rebuild so rect.height is valid on this frame.
        LayoutRebuilder.ForceRebuildLayoutImmediate(creditsContent);

        // --- Preserve the horizontal position you set in the inspector ---
        // We capture X *after* the layout rebuild so we get the settled
        // value even if a layout group nudged it. Y is animated by the
        // scroll, so it is not captured here.
        inspectorX = creditsContent.anchoredPosition.x;

        float startY = -Screen.height;
        float endY   = creditsContent.rect.height + Screen.height;

        // Park content at the start position, keeping the inspector X.
        creditsContent.anchoredPosition = new Vector2(inspectorX, startY);

        // ---------------------------------------------------------------
        // 2) FADE IN (+ optional simultaneous scroll)
        // ---------------------------------------------------------------

        float fadeElapsed   = 0f;
        float scrollElapsed = 0f;

        while (fadeElapsed < fadeDuration)
        {
            fadeElapsed += Time.unscaledDeltaTime;

            canvasGroup.alpha = Mathf.Clamp01(fadeElapsed / fadeDuration);

            if (scrollDuringFade)
            {
                scrollElapsed += Time.unscaledDeltaTime;
                float p = Mathf.Clamp01(scrollElapsed / scrollDuration);
                creditsContent.anchoredPosition =
                    new Vector2(inspectorX, Mathf.Lerp(startY, endY, p));
            }

            yield return null;
        }
        canvasGroup.alpha = 1f;

        // ---------------------------------------------------------------
        // 3) SCROLL (or finish the remaining scroll if it already started)
        // ---------------------------------------------------------------

        while (scrollElapsed < scrollDuration)
        {
            scrollElapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(scrollElapsed / scrollDuration);
            float currentY = Mathf.Lerp(startY, endY, progress);

            // Only Y changes — X stays exactly where you placed it.
            creditsContent.anchoredPosition = new Vector2(inspectorX, currentY);

            yield return null;
        }

        // Snap to the exact target position so we never end a pixel short.
        creditsContent.anchoredPosition = new Vector2(inspectorX, endY);

        // ---------------------------------------------------------------
        // 4) DONE
        // ---------------------------------------------------------------
        creditsRoutine = null;
        Debug.Log("Credits finished!");
    }

    public void SkipCredits()
    {
        if (creditsRoutine != null)
        {
            StopCoroutine(creditsRoutine);
            creditsRoutine = null;
        }

        // Reset state so credits can be replayed cleanly if needed.
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        // You can show your Main Menu here.
    }
}