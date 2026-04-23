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
                    if (wrong != correct && wrong >= 0 && !usedNumbers.Contains(wrong))
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

            if (LevelManager.Instance != null)
                LevelManager.Instance.OnCorrectAnswer();

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
            a = Random.Range(1, 9);
            b = Random.Range(1, 10 - a);
            q.formula = $"{a} + ? = {a + b}";
            q.correctAnswer = b;
        }
        else if (level == 2)
        {
            a = Random.Range(2, 12);
            b = Random.Range(1, a);
            if (Random.value > 0.5f) { q.formula = $"{a} + ? = {a + b}"; q.correctAnswer = b; }
            else { q.formula = $"{a} - ? = {a - b}"; q.correctAnswer = b; }
        }
        else if (level == 3)
        {
            if (Random.value > 0.6f)
            {
                a = Random.Range(2, 6); b = Random.Range(2, 6);
                q.formula = $"{a} x ? = {a * b}"; q.correctAnswer = b;
            }
            else
            {
                a = Random.Range(5, 18); b = Random.Range(1, a - 2);
                q.formula = $"{a} - ? = {a - b}"; q.correctAnswer = b;
            }
        }
        else if (level == 4)
        {
            a = Random.Range(2, 8); b = Random.Range(2, 8);
            if (Random.value > 0.5f) { q.formula = $"{a * b} / ? = {a}"; q.correctAnswer = b; }
            else { q.formula = $"{a} x ? = {a * b}"; q.correctAnswer = b; }
        }
        else
        {
            a = Random.Range(3, 10); b = Random.Range(2, 10);
            switch (Random.Range(0, 4))
            {
                case 0: q.formula = $"{a} + ? = {a + b}"; q.correctAnswer = b; break;
                case 1: q.formula = $"{a + b} - ? = {a}"; q.correctAnswer = b; break;
                case 2: q.formula = $"{a} x ? = {a * b}"; q.correctAnswer = b; break;
                default: q.formula = $"{a * b} / ? = {a}"; q.correctAnswer = b; break;
            }
        }

        return q;
    }
}