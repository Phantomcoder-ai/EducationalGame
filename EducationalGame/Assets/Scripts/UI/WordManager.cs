using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class WordManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI goalWordText;    // показывает слово или R_B_
    public TextMeshProUGUI currentWordText; // показывает собранные буквы
    public TextMeshProUGUI scoreText;

    [Header("Настройки")]
    public bool strictOrder = false;   // строгий порядок сборки
    public bool hiddenLetters = false; // скрытые буквы

    // Списки слов по уровням
    [System.Serializable]
    public class LevelWordList
    {
        public int level;
        public List<string> words = new List<string>();
    }

    private List<LevelWordList> wordLists = new List<LevelWordList>();
    private List<string> usedWords = new List<string>();
    private string targetWord = "";
    private List<char> collectedLetters = new List<char>();
    private int nextExpectedIndex = 0; // для строгого порядка

    void Awake()
    {
        InitializeDefaultWords();
    }

    void Start()
    {
        SetDifficulty(1);
    }

    void InitializeDefaultWords()
    {

        // Уровень 1 — 3-4 буквы
        wordLists.Add(new LevelWordList
        {
            level = 1,
            words = new List<string> {
                "KOT", "PSY", "DOM", "LAS", "RYB", "MOR",
                "GRA", "ZAB", "LOD", "SAN"
            }
        });

        // Уровень 2 — 4-5 букв
        wordLists.Add(new LevelWordList
        {
            level = 2,
            words = new List<string> {
                "RYBA", "WODA", "RANE", "POLE", "MOST",
                "RZEK", "OKNO", "DACH", "PTAK", "ZIMA"
            }
        });

        // Уровень 3 — 5-6 букв
        wordLists.Add(new LevelWordList
        {
            level = 3,
            words = new List<string> {
                "WEDKA", "MORZE", "ZAMEK", "KWIAT", "OWOCA",
                "LISCI", "BRZEG", "STAWA", "TRATW", "GLOWA"
            }
        });

        // Уровень 4 — 6-7 букв
        wordLists.Add(new LevelWordList
        {
            level = 4,
            words = new List<string> {
                "JEZIORO", "WĘDKARZ", "RYBAKÓW", "PŁYWAKA",
                "KACZORA", "ŻAGLOWI", "STAWIAJ", "GŁĘBOKI"
            }
        });

        // Уровень 5 — 7+ букв
        wordLists.Add(new LevelWordList
        {
            level = 5,
            words = new List<string> {
                "MATEMATYKA", "PRZYGODA", "WEDKARSKI",
                "JEZIOROUS", "PODWODNY", "GLĘBOKOŚĆ"
            }
        });
    }

    public void SetDifficulty(int level)
    {
        strictOrder = level >= 3;
        hiddenLetters = level >= 4;
        usedWords.Clear(); // новый уровень — новые слова
        LoadNewWord(level);
    }

    void LoadNewWord(int level)
    {
        LevelWordList list = wordLists.Find(w => w.level == level);

        if (list == null)
        {
            for (int i = level; i >= 1; i--)
            {
                list = wordLists.Find(w => w.level == i);
                if (list != null) break;
            }
        }

        if (list == null || list.words.Count == 0)
        {
            Debug.LogError($"WordManager: нет слов для уровня {level}!");
            return;
        }

        // Если все слова уровня использованы — сбрасываем историю
        if (usedWords.Count >= list.words.Count)
        {
            Debug.Log("Все слова использованы, сбрасываем историю!");
            usedWords.Clear();
        }

        // Берём только неиспользованные слова
        List<string> available = list.words.FindAll(w => !usedWords.Contains(w.ToUpper()));

        targetWord = available[Random.Range(0, available.Count)].ToUpper();
        usedWords.Add(targetWord);

        collectedLetters.Clear();
        nextExpectedIndex = 0;

        UpdateUI();
        SpawnLettersForWord();

        Debug.Log($"Новое слово: {targetWord} | Порядок: {strictOrder} | Скрытые: {hiddenLetters}");
    }

    void SpawnLettersForWord()
    {
        // Обновляем рыб через FishManager
        FishManager fishManager = FindAnyObjectByType<FishManager>();
        if (fishManager != null)
            fishManager.RefreshFishForNewWord(targetWord);
    }

    void UpdateUI()
    {
        // Показываем цель
        if (goalWordText != null)
            goalWordText.text = GetGoalDisplay();

        // Показываем собранные буквы
        if (currentWordText != null)
            currentWordText.text = GetCurrentDisplay();
    }

    string GetGoalDisplay()
    {
        if (!hiddenLetters)
            return targetWord;

        string display = "";
        for (int i = 0; i < targetWord.Length; i++)
            display += (i % 2 == 1) ? "_" : targetWord[i].ToString();
        return display;
    }

    public bool AddLetter(string letter)
    {
        char ch = letter.ToUpper()[0];

        if (strictOrder)
        {
            if (nextExpectedIndex >= targetWord.Length)
                return false;

            char expected = targetWord[nextExpectedIndex];
            if (ch != expected)
            {
                Debug.Log($"Не та буква! Ожидалась {expected}, поймали {ch}");
                if (HealthManager.Instance != null)
                    HealthManager.Instance.TakeDamage(1);
                if (LevelManager.Instance != null)
                    LevelManager.Instance.OnWrongAnswer();
                return false;
            }

            collectedLetters.Add(ch);
            nextExpectedIndex++;
        }
        else
        {
            // Считаем сколько раз буква встречается в слове
            int needed = 0;
            foreach (char c in targetWord) if (c == ch) needed++;

            // Считаем сколько уже поймано
            int caught = 0;
            foreach (char c in collectedLetters) if (c == ch) caught++;

            if (needed == 0 || caught >= needed)
            {
                Debug.Log($"Буква {ch} не нужна или уже собрана!");
                if (HealthManager.Instance != null)
                    HealthManager.Instance.TakeDamage(1);
                if (LevelManager.Instance != null)
                    LevelManager.Instance.OnWrongAnswer();
                return false;
            }

            // Просто добавляем букву в список — порядок не важен
            collectedLetters.Add(ch);
        }

        UpdateUI();

        if (IsWordComplete())
            OnWordComplete();

        return true;
    }

    bool IsWordComplete()
    {
        if (strictOrder)
            return nextExpectedIndex >= targetWord.Length;

        // Свободный порядок — сравниваем отсортированные списки
        if (collectedLetters.Count != targetWord.Length) return false;

        List<char> sortedCollected = new List<char>(collectedLetters);
        List<char> sortedTarget = new List<char>(targetWord.ToCharArray());
        sortedCollected.Sort();
        sortedTarget.Sort();

        for (int i = 0; i < sortedTarget.Count; i++)
            if (sortedCollected[i] != sortedTarget[i]) return false;

        return true;
    }

    string GetCurrentDisplay()
    {
        string display = "";
        List<char> temp = new List<char>(collectedLetters);

        for (int i = 0; i < targetWord.Length; i++)
        {
            char needed = targetWord[i];
            if (temp.Contains(needed))
            {
                display += needed;
                temp.Remove(needed);
            }
            else
            {
                display += "_";
            }
            if (i < targetWord.Length - 1)
                display += " ";
        }
        return display;
    }

    void OnWordComplete()
    {
        Debug.Log($"Слово {targetWord} собрано!");

        if (currentWordText != null)
            currentWordText.text = targetWord + " [OK]";

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnCorrectAnswer();
            if (scoreText != null)
                scoreText.text = "Score: " + LevelManager.Instance.totalScore;
        }

        // Небольшая пауза перед следующим словом
        Invoke(nameof(NextWord), 1.5f);
    }

    void NextWord()
    {
        int level = LevelManager.Instance != null ? LevelManager.Instance.currentLevel : 1;
        LoadNewWord(level);
    }

    public string GetTargetWord() => targetWord;
}