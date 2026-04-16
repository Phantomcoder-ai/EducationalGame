using UnityEngine;
using TMPro;

public class WordManager : MonoBehaviour
{

    [Header("Ссылка на спавнер")]
    public FishManager fishManager; // Перетащи FishManager сюда в инспекторе

    [Header("Настройки сложности")]
    public int wordsBeforeShark = 2; // Акула появится после 2-го слова

    public string[] wordList = { "MAMA", "KUTASIK", "EPSZTEJN", "URAN", "MADURA", "STOJAKRONALDU" }; // Список слов для игры
    private int currentWordIndex = 0;

    public TextMeshProUGUI DisplayWord; // Ссылка на наш WordText из UI
    public TextMeshProUGUI GoalWord;

    private int currentProgress = 0;

    void Start()
    {
        UpdateUI();
    }

    public string GetCurrentWord()
    {
        return wordList[currentWordIndex];
    }

    public bool AddLetter(string letter)
    {
        string targetWord = GetCurrentWord();

        // Проверяем, совпадает ли буква с текущей нужной в слове
        if (currentProgress < targetWord.Length &&
            letter.Trim().ToUpper() == targetWord[currentProgress].ToString().ToUpper())
        {
            currentProgress++;
            UpdateUI();

            if(currentProgress >= targetWord.Length)
            {
                Debug.Log("Слово " + targetWord + " собрано! Переходим к следующему слову.");
                Invoke("NextWord", 1.0f);
            }
            Debug.Log("Буква верная! Прогресс: " + currentProgress);
            return true;
        }
        else
        {
            Debug.Log("Не та буква. Нужно: " + targetWord[currentProgress]);
            if (HealthManager.Instance != null)
            {
                HealthManager.Instance.TakeDamage(1);
            }
            return false;
        }
    }

    void NextWord()
    {
        currentWordIndex++;
        currentProgress = 0;
        if(currentWordIndex < wordList.Length)
        {
            string nextWord = GetCurrentWord();
            Debug.Log("Следующее слово: " + nextWord);
             UpdateUI();
            if(fishManager != null)
            {
                fishManager.RefreshFishForNewWord(nextWord);

                // ПРОВЕРКА: Пора ли выпускать акулу?
                if (currentWordIndex >= wordsBeforeShark)
                {
                    fishManager.SpawnShark();
                }
            }
            
        }
        else
        {
            DisplayWord.text = "WSZYSTKIE S?OWA ZEBRANE!";
            Debug.Log("Все слова собраны! Уровень пройден!");
        }
    }

    void UpdateUI()
    {
        string targetWord = GetCurrentWord();
        if(GoalWord != null)
        {
            GoalWord.text = targetWord.ToUpper();
        }

        string display = "";
        for (int i = 0; i < targetWord.Length; i++)
        {
            if (i < currentProgress) display += targetWord[i] + " ";
            else display += "_ ";
        }
        DisplayWord.text = display;
    }
}