using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MathManager : MonoBehaviour
{
    [Header("UI Элементы")]
    public TextMeshProUGUI currentQuestionText;  // текущий вопрос с "?"
    public TextMeshProUGUI completedQuestionText; // решённый пример
    public TextMeshProUGUI scoreText;

    [Header("Настройки ответов")]
    public int correctFishCount = 2; // сколько рыб будут иметь правильный ответ

    [Header("Система здоровья")]
    public HealthManager healthManager;

    [System.Serializable]
    public struct MathQuestion
    {
        public string formula;
        public int correctAnswer;
    }

    private MathQuestion currentQuestion;

    void Start()
    {
        if (scoreText != null) scoreText.text = "Score: 0";
        SetDifficulty(1);
    }

    public void ShowQuestion()
    {
        if (currentQuestionText != null)
            currentQuestionText.text = currentQuestion.formula;

        if (completedQuestionText != null)
            completedQuestionText.text = "";

        UpdateFishAnswers(currentQuestion.correctAnswer);
    }

    void UpdateFishAnswers(int correct)
    {
        FishMath[] allFish;
#if UNITY_2021_2_OR_NEWER
        allFish = Object.FindObjectsByType<FishMath>(FindObjectsSortMode.None);
#else
    allFish = GameObject.FindObjectsOfType<FishMath>();
#endif
        if (allFish == null || allFish.Length == 0)
        {
            Debug.LogWarning("MathManager: рыбы с FishMath не найдены.");
            return;
        }

        // Выбираем несколько случайных индексов для правильных рыб
        List<int> luckyIndices = new List<int>();
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < allFish.Length; i++)
            availableIndices.Add(i);

        int count = Mathf.Min(correctFishCount, allFish.Length);
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, availableIndices.Count);
            luckyIndices.Add(availableIndices[randomIndex]);
            availableIndices.RemoveAt(randomIndex);
        }

        // Раздаём ответы
        List<int> usedNumbers = new List<int> { correct };

        for (int i = 0; i < allFish.Length; i++)
        {
            if (luckyIndices.Contains(i))
            {
                allFish[i].SetMathValue(correct);
            }
            else
            {
                int wrong = 0;
                int attempts = 0;
                bool foundUnique = false;

                while (attempts < 100)
                {
                    wrong = correct + Random.Range(-10, 11);
                    if (wrong != correct && !usedNumbers.Contains(wrong))
                    {
                        foundUnique = true;
                        break;
                    }
                    attempts++;
                }

                if (!foundUnique) wrong = correct + i + 1;
                usedNumbers.Add(wrong);
                allFish[i].SetMathValue(wrong);
            }
        }
    }

    public bool CheckAnswer(int playerAnswer)
    {
        if (playerAnswer == currentQuestion.correctAnswer)
        {
            // Показываем решённый пример
            string completed = currentQuestion.formula.Replace("?", playerAnswer.ToString());
            if (completedQuestionText != null)
                completedQuestionText.text = completed + " [OK]";
            if (currentQuestionText != null)
                currentQuestionText.text = "";

            if (LevelManager.Instance != null) { 
                LevelManager.Instance.OnCorrectAnswer();
                if (scoreText != null)
                    scoreText.text = "Score: " + LevelManager.Instance.totalScore;
            }
                

            StartCoroutine(ShowNextQuestionDelayed(1.2f));
           
            return true;
        }
        else
        {
            if (LevelManager.Instance != null)
                LevelManager.Instance.OnWrongAnswer();
            if (healthManager != null)
                healthManager.TakeDamage(1);
        }
        return false;
    }

    private System.Collections.IEnumerator ShowNextQuestionDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        GenerateAndShow();
    }

    void GenerateAndShow()
    {
        int level = LevelManager.Instance != null ? LevelManager.Instance.currentLevel : 1;
        currentQuestion = GenerateQuestion(level);
        ShowQuestion();
    }

    public void SetDifficulty(int level)
    {
        currentQuestion = GenerateQuestion(level);
        ShowQuestion();
    }

    MathQuestion GenerateQuestion(int level)
    {
        MathQuestion q = new MathQuestion();
        int a, b;

        if (level == 1)
        {
            a = Random.Range(1, 11);
            b = Random.Range(1, 11);
            if (Random.value > 0.5f)
            {
                q.formula = $"{a} + ? = {a + b}";
                q.correctAnswer = b;
            }
            else
            {
                // b может быть больше a — ответ будет отрицательным
                q.formula = $"{a} - ? = {a - b}";
                q.correctAnswer = b;
            }
        }
        else if (level == 2)
        {
            // Плюс/минус, числа до 100
            a = Random.Range(1, 51);
            b = Random.Range(1, 51);
            if (Random.value > 0.5f)
            {
                q.formula = $"{a} + ? = {a + b}";
                q.correctAnswer = b;
            }
            else
            {
                int big = Mathf.Max(a, b);
                int small = Mathf.Min(a, b);
                q.formula = $"{big} - ? = {big - small}";
                q.correctAnswer = small;
            }
        }
        else if (level == 3)
        {
            // Плюс/минус/умножение, числа от 1 до 10
            int type = Random.Range(0, 3);
            a = Random.Range(1, 11);
            b = Random.Range(1, 11);

            switch (type)
            {
                case 0:
                    q.formula = $"{a} + ? = {a + b}";
                    q.correctAnswer = b;
                    break;
                case 1:
                    int big = Mathf.Max(a, b);
                    int small = Mathf.Min(a, b);
                    q.formula = $"{big} - ? = {big - small}";
                    q.correctAnswer = small;
                    break;
                case 2:
                    q.formula = $"{a} x ? = {a * b}";
                    q.correctAnswer = b;
                    break;
            }
        }
        else if (level == 4)
        {
            // Умножение (1-10) и деление (числа до 100)
            if (Random.value > 0.5f)
            {
                // Умножение
                a = Random.Range(2, 11);
                b = Random.Range(2, 11);
                q.formula = $"{a} x ? = {a * b}";
                q.correctAnswer = b;
            }
            else
            {
                // Деление — берём красивые числа
                // делитель от 2 до 12, результат от 2 до 12
                b = Random.Range(2, 13); // делитель
                int result = Random.Range(2, 13); // результат
                a = b * result; // делимое
                q.formula = $"{a} / ? = {result}";
                q.correctAnswer = b;
            }
        }
        else // level 5
        {
            // Всё вместе, усложнённые числа
            int type = Random.Range(0, 5);

            switch (type)
            {
                case 0:
                    // Сложение до 200
                    a = Random.Range(10, 151);
                    b = Random.Range(10, 151);
                    q.formula = $"{a} + ? = {a + b}";
                    q.correctAnswer = b;
                    break;
                case 1:
                    // Вычитание до 200
                    a = Random.Range(50, 201);
                    b = Random.Range(10, a);
                    q.formula = $"{a} - ? = {a - b}";
                    q.correctAnswer = b;
                    break;
                case 2:
                    // Умножение до 12
                    a = Random.Range(2, 13);
                    b = Random.Range(2, 13);
                    q.formula = $"{a} x ? = {a * b}";
                    q.correctAnswer = b;
                    break;
                case 3:
                    // Деление, красивые числа
                    b = Random.Range(2, 13);
                    int res = Random.Range(2, 13);
                    a = b * res;
                    q.formula = $"{a} / ? = {res}";
                    q.correctAnswer = b;
                    break;
                case 4:
                    // Двухшаговое выражение: a + b*c = ?
                    // Показываем как "a + b x c = ?" где ? это результат b*c
                    int m = Random.Range(2, 8);
                    int n = Random.Range(2, 8);
                    int sum = Random.Range(5, 20);
                    q.formula = $"{sum} + {m} x ? = {sum + m * n}";
                    q.correctAnswer = n;
                    break;
            }
        }

        return q;
    }
}