using UnityEngine;
using TMPro;
using System.Collections;

public class LevelUpPopup : MonoBehaviour
{
    public static LevelUpPopup Instance;

    [Header("UI")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI scoreText;

    [Header("Настройки")]
    public float displayDuration = 2.5f;
    public float fadeDuration = 0.4f;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        Instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f; // невидим но активен
    }

    public void Show(int newLevel, int score)
    {
        if (levelText != null)
            levelText.text = newLevel >= 6 ? "ПОБЕДА!" : $"УРОВЕНЬ {newLevel}";
        if (scoreText != null)
            scoreText.text = "Очки: " + score;
        // Показываем
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(ShowRoutine());
    }

    IEnumerator ShowRoutine()
    {
        
        

        // Плавное появление
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Ждём
        yield return new WaitForSeconds(displayDuration);

        // Плавное исчезновение
        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        gameObject.SetActive(false);

        // Сигналим LevelManager что можно продолжать
        LevelManager.Instance?.OnLevelUpPopupFinished();
    }
}