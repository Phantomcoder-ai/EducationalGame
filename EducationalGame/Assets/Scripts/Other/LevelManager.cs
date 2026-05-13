using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public enum Mode { Math, Word }

    [Header("Режим игры")]
    public Mode gameMode = Mode.Math;

    [Header("Настройки уровней")]
    public int correctAnswersPerLevelMath = 10;
    public int correctAnswersPerLevelWord = 5;
    public int maxLevels = 5;

    [Header("Триггеры внутри уровня")]
    public int sharkSpawnAnswer = 2;      // после какого ответа появляется акула
    public int darknessStartAnswer = 1;   // после какого ответа включается темнота

    [Header("Текущее состояние")]
    public int currentLevel = 1;
    public int correctAnswersThisLevel = 0;
    public int totalScore = 0;
    public int comboCount = 0;

    [Header("Ссылки")]
    public FishManager fishManager;
    public MathManager mathManager;   // только для Math режима
    public WordManager wordManager;   // только для Word режима
    public TimerController timerController;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ApplyLevelSettings();
    }

    public void OnCorrectAnswer()
    {
        correctAnswersThisLevel++;

        // Комбо очки
        comboCount++;
        int points = Mathf.Min(10 * comboCount, 80);
        totalScore += points;

        GameSessionData.score = totalScore;
        GameSessionData.combo = comboCount;

        // Триггеры внутри уровня
        if (correctAnswersThisLevel == sharkSpawnAnswer)
            fishManager?.SpawnShark();

        int answersNeeded = gameMode == Mode.Math ? correctAnswersPerLevelMath : correctAnswersPerLevelWord;
        if (correctAnswersThisLevel >= answersNeeded)
            NextLevel();
    }

    public void OnWrongAnswer()
    {
        comboCount = 0;
    }

    void NextLevel()
    {
        Debug.Log("NextLevel вызван! LevelUpPopup.Instance = " + LevelUpPopup.Instance);

        correctAnswersThisLevel = 0;
        comboCount = 0;

        // Проверяем ПОСЛЕ того как должны были бы перейти
        if (currentLevel >= maxLevels)
        {
            GameSessionData.isVictory = true;
            GameSessionData.lastSceneName = SceneManager.GetActiveScene().name;
            Time.timeScale = 1f;
            SceneManager.LoadScene("ResultScene");
            return;
        }

        currentLevel++;

        if (timerController != null)
            timerController.StopTimer();
        AudioManager.Instance?.PlayLevelUp();
        if (LevelUpPopup.Instance != null)
            LevelUpPopup.Instance.Show(currentLevel, totalScore);
        else
            OnLevelUpPopupFinished();
    }

    // Вызывается когда попап закрылся
    public void OnLevelUpPopupFinished()
    {
        ApplyLevelSettings();
        ResetLevelTriggers();
    }

    void ApplyLevelSettings()
    {
        // Скорость рыб — одинаково для обоих режимов
        float fishSpeed = 1f + (currentLevel - 1) * 0.4f;
        SetAllFishSpeed(fishSpeed);

        // Таймер с 3 уровня — одинаково для обоих режимов
        if (timerController != null)
        {
            if (currentLevel >= 3)
            {
                float duration = 20f + (currentLevel - 3) * 5f;
                timerController.SetTimer(duration);
                timerController.gameObject.SetActive(true);
            }
            else
            {
                timerController.gameObject.SetActive(false);
            }
        }

        // Сложность — зависит от режима
        if (gameMode == Mode.Math && mathManager != null)
            mathManager.SetDifficulty(currentLevel);

        if (gameMode == Mode.Word && wordManager != null)
            wordManager.SetDifficulty(currentLevel);

        Debug.Log($"Уровень {currentLevel} начался! Режим: {gameMode}");
    }

    void ResetLevelTriggers()
    {

        // Выключаем темноту
        DarknessController darkness = FindAnyObjectByType<DarknessController>();
        if (darkness != null) darkness.DisableDarkness();

        // Сбрасываем флаг камеры
        CameraController cam = FindAnyObjectByType<CameraController>();
        if (cam != null) cam.darknessTriggered = false;

        // Убираем акулу
        GameObject shark = GameObject.FindGameObjectWithTag("Shark");
        if (shark != null) Destroy(shark);
    }

    void SetAllFishSpeed(float speed)
    {
        FishMovement[] allFish = FindObjectsByType<FishMovement>(FindObjectsSortMode.None);
        foreach (var fish in allFish)
            fish.speed = speed;
    }
}