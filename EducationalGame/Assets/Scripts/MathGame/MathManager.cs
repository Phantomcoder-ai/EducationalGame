using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MathManager : MonoBehaviour
{
    [Header("UI Элементы")]
    public TextMeshProUGUI formulaText;
    public TextMeshProUGUI scoreText;

    [Header("Текущий собираемый пример (на боковой панели)")]
    public TextMeshProUGUI currentExpressionText; // сюда притащи текст на боковой панели, который должен показывать заменённый "?"

    [Header("Система здоровья")]
    public HealthManager healthManager; // Сюда в инспекторе перетащи объект HealthBar

    [Header("Боковая панель")]
    public Transform sidePanelContainer; // Сюда перетащи объект панели (Content)
    public GameObject mathHistoryPrefab;  // Префаб текста (строчка примера)

    [System.Serializable]
    public struct MathQuestion
    {
        public string formula;   // Сама строка, например "10 + ? = 15"
        public int correctAnswer; // Ответ, который должен быть на рыбе (в данном случае 5)
    }

    [Header("Список твоих выражений")]
    public List<MathQuestion> questions = new List<MathQuestion>();

    private int currentQuestionIndex = 0;
    

    void Start()
    {
        // Инициализация счёта
        
        if (scoreText != null) scoreText.text = "Score: 0";

        if (questions.Count > 0)
        {
            ShowQuestion();
        }
        else
        {
            Debug.LogError("MathManager: список вопросов пуст! Добавь их в Инспекторе.");
            if (currentExpressionText != null)
                currentExpressionText.text = "Нет примеров";
        }
    }

    public void ShowQuestion()
    {
        if (questions.Count == 0)
        {
            Debug.LogWarning("MathManager.ShowQuestion: нет вопросов.");
            return;
        }

        // Берем текущий вопрос из списка
        MathQuestion q = questions[currentQuestionIndex];

        if (formulaText != null) formulaText.text = q.formula;
        else Debug.LogWarning("MathManager: formulaText не назначен в инспекторе.");

        // Обновляем отображение текущего собираемого выражения на боковой панели (с вопросительным знаком пока)
        if (currentExpressionText != null)
        {
            currentExpressionText.text = q.formula;
        }
        else
        {
            Debug.LogWarning("MathManager: currentExpressionText не назначен в инспекторе.");
        }

        // Раздаем ответы рыбам
        UpdateFishAnswers(q.correctAnswer);
    }

    void UpdateFishAnswers(int correct)
    {
        // Используем современный метод (если его нет в вашей версии Unity — замените на FindObjectsOfType<FishMath>())
        FishMath[] allFish;
#if UNITY_2021_2_OR_NEWER
        allFish = Object.FindObjectsByType<FishMath>(FindObjectsSortMode.None);
#else
        allFish = GameObject.FindObjectsOfType<FishMath>();
#endif
        if (allFish == null || allFish.Length == 0)
        {
            Debug.LogWarning("MathManager.UpdateFishAnswers: рыбы с FishMath не найдены в сцене.");
            return;
        }

        List<int> usedNumbers = new List<int> { correct };
        int luckyIndex = Random.Range(0, allFish.Length);

        for (int i = 0; i < allFish.Length; i++)
        {
            if (i == luckyIndex)
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
        if (questions.Count == 0) return false;

        int questionIndexAtCheck = currentQuestionIndex;

        if (playerAnswer == questions[questionIndexAtCheck].correctAnswer)
        {
            string completedFormula = questions[questionIndexAtCheck].formula
                .Replace("?", playerAnswer.ToString());

            // Показываем замену на боковой панели
            if (currentExpressionText != null)
                currentExpressionText.text = completedFormula + " ✓";

            // Добавляем в историю
            AddToHistory(questions[questionIndexAtCheck].formula, playerAnswer);

            /*if (LevelManager.Instance != null)
                LevelManager.Instance.OnCorrectAnswer();
            else
            {
                // фолбэк если LevelManager не назначен
                score++;
                if (scoreText != null) scoreText.text = "Score: " + score;
            }*/

            // Задержка перед следующим вопросом, чтобы "?" успела замениться на экране
            StartCoroutine(ShowNextQuestionDelayed(1.2f));
            
            return true;


        }
        else
        {
            /*if (LevelManager.Instance != null)
                LevelManager.Instance.OnWrongAnswer();*/

            if (healthManager != null)
                healthManager.TakeDamage(1);
        }
        return false;
    }

    private System.Collections.IEnumerator ShowNextQuestionDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowQuestion();
    }

    public void AddToHistory(string formula, int answer)
    {
        if (sidePanelContainer == null)
        {
            Debug.LogWarning("MathManager.AddToHistory: sidePanelContainer не назначен.");
            return;
        }
        if (mathHistoryPrefab == null)
        {
            Debug.LogWarning("MathManager.AddToHistory: mathHistoryPrefab не назначен.");
            return;
        }

        // Создаем новую строчку в списке
        GameObject newEntry = Instantiate(mathHistoryPrefab, sidePanelContainer);

        // Пытаемся найти TMP-текст внутри префаба (включая дочерние)
        TextMeshProUGUI entryText = newEntry.GetComponentInChildren<TextMeshProUGUI>(true);
        if (entryText == null)
        {
            // Попытаться взять компонент на корне
            entryText = newEntry.GetComponent<TextMeshProUGUI>();
        }

        if (entryText == null)
        {
            Debug.LogError("MathManager.AddToHistory: в mathHistoryPrefab нет TextMeshProUGUI. Нельзя заполнить запись истории.");
            return;
        }

        // Заменяем вопрос на ответ (например "3 + ? = 7" станет "3 + 4 = 7")
        string completedFormula = formula.Replace("?", answer.ToString());
        entryText.text = completedFormula + " ✓";

        // Поставить последним в списке
        newEntry.transform.SetAsLastSibling();

        // Опционально: удалять самую старую запись, если их стало слишком много (> 6)
        if (sidePanelContainer.childCount > 6)
        {
            Destroy(sidePanelContainer.GetChild(0).gameObject);
        }

        Debug.Log("MathManager.AddToHistory: добавлена запись: " + completedFormula);
    }

    public void SetDifficulty(int level)
    {
        questions.Clear();

        for (int i = 0; i < 10; i++)
        {
            MathQuestion q = GenerateQuestion(level);
            questions.Add(q);
        }

        currentQuestionIndex = 0;
        ShowQuestion();
    }

    MathQuestion GenerateQuestion(int level)
    {
        MathQuestion q = new MathQuestion();
        int a, b;

        if (level == 1)
        {
            // Только сложение, числа до 10
            a = Random.Range(1, 9);
            b = Random.Range(1, 10 - a);
            q.formula = $"{a} + ? = {a + b}";
            q.correctAnswer = b;
        }
        else if (level == 2)
        {
            // Сложение и вычитание, числа до 15
            a = Random.Range(2, 12);
            b = Random.Range(1, a);
            bool isAdd = Random.value > 0.5f;
            if (isAdd)
            {
                q.formula = $"{a} + ? = {a + b}";
                q.correctAnswer = b;
            }
            else
            {
                q.formula = $"{a} - ? = {a - b}";
                q.correctAnswer = b;
            }
        }
        else if (level == 3)
        {
            // Числа до 20, появляется умножение
            bool isMult = Random.value > 0.6f;
            if (isMult)
            {
                a = Random.Range(2, 6);
                b = Random.Range(2, 6);
                q.formula = $"{a} x ? = {a * b}";
                q.correctAnswer = b;
            }
            else
            {
                a = Random.Range(5, 18);
                b = Random.Range(1, a - 2);
                q.formula = $"{a} - ? = {a - b}";
                q.correctAnswer = b;
            }
        }
        else if (level == 4)
        {
            // Умножение и деление
            a = Random.Range(2, 8);
            b = Random.Range(2, 8);
            bool isDiv = Random.value > 0.5f;
            if (isDiv)
            {
                q.formula = $"{a * b} / ? = {a}";
                q.correctAnswer = b;
            }
            else
            {
                q.formula = $"{a} x ? = {a * b}";
                q.correctAnswer = b;
            }
        }
        else // level 5
        {
            // Всё вместе, числа до 50
            int type = Random.Range(0, 4);
            a = Random.Range(3, 10);
            b = Random.Range(2, 10);
            switch (type)
            {
                case 0: q.formula = $"{a} + ? = {a + b}"; q.correctAnswer = b; break;
                case 1: q.formula = $"{a + b} - ? = {a}"; q.correctAnswer = b; break;
                case 2: q.formula = $"{a} x ? = {a * b}"; q.correctAnswer = b; break;
                case 3: q.formula = $"{a * b} / ? = {a}"; q.correctAnswer = b; break;
            }
        }

        return q;
    }
}