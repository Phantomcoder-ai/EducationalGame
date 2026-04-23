using UnityEngine;
using TMPro;

public class TimerController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI timerText;

    [Header("Настройки")]
    public Color normalColor = Color.white;
    public Color urgentColor = Color.red;
    public float urgentThreshold = 5f;

    private float timeLeft;
    private bool isRunning = false;

    public void SetTimer(float duration)
    {
        timeLeft = duration;
        isRunning = false;
        UpdateUI();
    }

    public void StartTimer()
    {
        if (timeLeft > 0f)
            isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        if (LevelManager.Instance == null) return;

        if (LevelManager.Instance.currentLevel >= 3)
        {
            float duration = 20f + (LevelManager.Instance.currentLevel - 3) * 5f;
            SetTimer(duration);
        }
    }

    void Update()
    {
        if (!isRunning) return;

        timeLeft -= Time.deltaTime;
        timeLeft = Mathf.Max(timeLeft, 0f);
        UpdateUI();

        if (timeLeft <= 0f)
            OnTimerExpired();
    }

    void UpdateUI()
    {
        if (timerText == null) return;
        timerText.text = Mathf.CeilToInt(timeLeft).ToString();
        timerText.color = timeLeft <= urgentThreshold ? urgentColor : normalColor;
    }

    void OnTimerExpired()
    {
        isRunning = false;

        // Поднимаем крючок
        HookController hook = FindAnyObjectByType<HookController>();
        if (hook != null) hook.ForceReturn();

        // Минус жизнь
        if (HealthManager.Instance != null)
            HealthManager.Instance.TakeDamage(1);

        // Сброс комбо
        if (LevelManager.Instance != null)
            LevelManager.Instance.OnWrongAnswer();

        Debug.Log("Время вышло!");
    }
}