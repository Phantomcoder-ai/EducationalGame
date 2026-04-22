/*using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Настройки уровней")]
    public int correctAnswersPerLevel = 10;
    public int maxLevels = 5;

    [Header("Текущее состояние")]
    public int currentLevel = 1;
    public int correctAnswersThisLevel = 0;
    public int totalScore = 0;
    public int comboCount = 0;

    [Header("Ссылки")]
    public FishManager fishManager;
    public MathManager mathManager;
    public DarknessController darknessController; // создадим позже
    public TimerController timerController;       // создадим позже

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ApplyLevelSettings();
    }

    // Вызывается из MathManager или WordManager при правильном ответе
    public void OnCorrectAnswer()
    {
        correctAnswersThisLevel++;

        // Комбо очки
        comboCount++;
        int points = 10 * comboCount; // 10, 20, 30, 40...
        if (comboCount >= 4) points = 80; // кап на комбо
        totalScore += points;

        GameSessionData.score = totalScore;
        GameSessionData.combo = comboCount;

        // Триггеры внутри уровня
        if (correctAnswersThisLevel == 2)
            fishManager.SpawnShark();

        if (correctAnswersThisLevel == 5 && darknessController != null)
            darknessController.EnableDarkness();

        // Переход на следующий уровень
        if (correctAnswersThisLevel >= correctAnswersPerLevel)
            NextLevel();
    }

    // Вызывается при неправильном ответе
    public void OnWrongAnswer()
    {
        comboCount = 0; // сброс комбо
    }

    void NextLevel()
    {
        correctAnswersThisLevel = 0;
        comboCount = 0;

        if (currentLevel >= maxLevels)
        {
            // Победа!
            GameSessionData.isVictory = true;
            GameSessionData.lastSceneName = SceneManager.GetActiveScene().name;
            Time.timeScale = 1f;
            SceneManager.LoadScene("ResultScene");
            return;
        }

        currentLevel++;
        ApplyLevelSettings();
        ResetLevelTriggers();
    }

    void ApplyLevelSettings()
    {
        // Скорость рыб
        float fishSpeed = 1f + (currentLevel - 1) * 0.4f;
        SetAllFishSpeed(fishSpeed);

        // Таймер (с 3 уровня)
        if (timerController != null)
        {
            if (currentLevel >= 3)
            {
                float timerDuration = 20f + (currentLevel - 3) * 5f; // 20, 25, 30
                timerController.SetTimer(timerDuration);
                timerController.gameObject.SetActive(true);
            }
            else
            {
                timerController.gameObject.SetActive(false);
            }
        }

        // Сложность математики
        if (mathManager != null)
            mathManager.SetDifficulty(currentLevel);

        Debug.Log($"Уровень {currentLevel} начался!");
    }

    void ResetLevelTriggers()
    {
        // Убираем акулу и темноту при переходе на новый уровень
        if (darknessController != null)
            darknessController.DisableDarkness();

        // Акулу уничтожаем
        GameObject shark = GameObject.FindGameObjectWithTag("Shark");
        if (shark != null) Destroy(shark);
    }

    void SetAllFishSpeed(float speed)
    {
        FishMovement[] allFish = FindObjectsByType<FishMovement>(FindObjectsSortMode.None);
        foreach (var fish in allFish)
            fish.speed = speed;
    }
}*/