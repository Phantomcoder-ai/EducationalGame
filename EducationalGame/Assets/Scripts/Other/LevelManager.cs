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
    public int darknessLevel = 3; // с какого уровня включается темнота
    
    [Header("Триггеры внутри уровня")]
    public int sharkSpawnAnswer = 2;
    

    [Header("Текущее состояние")]
    public int currentLevel = 1;
    public int correctAnswersThisLevel = 0;
    public int totalScore = 0;
    public int comboCount = 0;

    [Header("Ссылки")]
    public FishManager fishManager;
    public MathManager mathManager;
    public WordManager wordManager;
    public TimerController timerController;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ApplyLevelSettings();
    }

    // Проверка — можно ли включать темноту
    public bool IsDarknessAllowed()
    {
        return currentLevel >= darknessLevel;
    }

    public void OnCorrectAnswer()
    {
        correctAnswersThisLevel++;

        comboCount++;
        int points = Mathf.Min(10 * comboCount, 80);
        totalScore += points;

        GameSessionData.score = totalScore;
        GameSessionData.combo = comboCount;

        if (correctAnswersThisLevel == sharkSpawnAnswer)
            fishManager?.SpawnShark();

        int answersNeeded = gameMode == Mode.Math
            ? correctAnswersPerLevelMath
            : correctAnswersPerLevelWord;

        if (correctAnswersThisLevel >= answersNeeded)
            NextLevel();
    }

    public void OnWrongAnswer()
    {
        comboCount = 0;
    }

    void NextLevel()
    {
        correctAnswersThisLevel = 0;
        comboCount = 0;

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

    public void OnLevelUpPopupFinished()
    {
        ApplyLevelSettings();
        ResetLevelTriggers();
    }

    void ApplyLevelSettings()
    {
        if (timerController != null)
        {
            Debug.Log("timerController найден! Уровень: " + currentLevel);
            if (currentLevel >= 2)
            {
                float duration = 20f + (currentLevel - 2) * 5f;
                timerController.SetTimer(duration);
                timerController.gameObject.SetActive(true);
                Debug.Log("Таймер включён!");
            }
            else
            {
                timerController.gameObject.SetActive(false);
                Debug.Log("Таймер выключен!");
            }
        }
        else
        {
            Debug.LogError("timerController НЕ НАЗНАЧЕН в LevelManager!");
        }

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

        // Убираем акулу
        GameObject shark = GameObject.FindGameObjectWithTag("Shark");
        if (shark != null) Destroy(shark);
    }
}