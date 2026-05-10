using UnityEngine;
using TMPro;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("UI")]
    public GameObject tipPanel;        // панель с подсказкой
    public TextMeshProUGUI tipText;    // текст подсказки

    [Header("Настройки")]
    public float fadeDuration = 0.3f;

    private CanvasGroup canvasGroup;
    private Coroutine currentCoroutine;

    // Флаги — чтобы одна подсказка не показывалась дважды
    private bool shownSpaceHint = false;
    private bool shownEnterHint = false;
    private bool shownSharkHint = false;
    private bool shownFuguHint = false;
    private bool shownDarknessHint = false;
    private bool shownGoldenHint = false;

    void Awake()
    {
        Instance = this;
        canvasGroup = tipPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = tipPanel.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        tipPanel.SetActive(false);
    }

    // Показать подсказку на N секунд
    public void ShowTip(string text, float duration = 5f)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(ShowTipRoutine(text, duration));
    }

    IEnumerator ShowTipRoutine(string text, float duration)
    {
        tipText.text = text;
        tipPanel.SetActive(true);

        // Появление
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(duration);

        // Исчезновение
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        tipPanel.SetActive(false);
    }

    // Скрыть сразу
    public void HideTip()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        canvasGroup.alpha = 0f;
        tipPanel.SetActive(false);
    }

    // ---- Триггеры подсказок ----

    public void OnGameStart()
    {
        if (shownSpaceHint) return;
        shownSpaceHint = true;
        ShowTip("Naciśnij SPACJĘ aby zarzucić wędkę!", 5f);
    }

    public void OnFishInRange()
    {
        if (shownEnterHint) return;
        shownEnterHint = true;
        ShowTip("Naciśnij ENTER aby złapać rybę!", 5f);
    }

    public void OnSharkAppeared()
    {
        if (shownSharkHint) return;
        shownSharkHint = true;
        ShowTip("Uwaga! Rekin kradnie rybę!", 5f);
    }

    public void OnFuguAppeared()
    {
        if (shownFuguHint) return;
        shownFuguHint = true;
        ShowTip("Uważaj na rybę-fugu! Zabiera serce!", 5f);
    }

    public void OnDarknessEnabled()
    {
        if (shownDarknessHint) return;
        shownDarknessHint = true;
        ShowTip("Ciemność! Trzymaj się przy świetle!", 5f);
    }

    public void OnGoldenFishAppeared()
    {
        if (shownGoldenHint) return;
        shownGoldenHint = true;
        ShowTip("Złota rybka! Złap ją — przywróci serce!", 5f);
    }
}