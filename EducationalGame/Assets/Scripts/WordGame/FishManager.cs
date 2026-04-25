using UnityEngine;
using System.Collections.Generic;

public class FishManager : MonoBehaviour
{
    public enum GameMode { Word, Math }

    [Header("Режим игры")]
    public GameMode gameMode = GameMode.Word;

    [Header("Префабы рыб")]
    public GameObject[] fishPrefabs;

    [Header("Акула")]
    public GameObject sharkPrefab;
    private GameObject activeShark;

    [Header("Словарный режим")]
    public string targetWord = "МАМА";
    public string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public int extraFishCount = 15;

    [Header("Математический режим")]
    public MathManager mathManager;
    public int mathFishCount = 15; // сколько рыб с числами спавнить

    

    void Start()
    {
        SpawnAllFish();
    }

    void SpawnAllFish()
    {
        if (gameMode == GameMode.Word)
        {
            SpawnWordFish();
        }
        else if (gameMode == GameMode.Math)
        {
            SpawnMathFish();
        }
    }

    // --- СЛОВАРНЫЙ РЕЖИМ ---
    void SpawnWordFish()
    {
        for (int i = 0; i < targetWord.Length; i++)
            CreateLetterFish(targetWord[i].ToString());

        for (int i = 0; i < extraFishCount; i++)
        {
            char randomChar = alphabet[Random.Range(0, alphabet.Length)];
            CreateLetterFish(randomChar.ToString());
        }
    }

    void CreateLetterFish(string letter)
    {
        GameObject newFish = SpawnFishAtRandom();
        FishLetter fishLetterScript = newFish.GetComponentInChildren<FishLetter>();
        if (fishLetterScript != null)
            fishLetterScript.SetupLetter(letter);
    }

    // --- МАТЕМАТИЧЕСКИЙ РЕЖИМ ---
    void SpawnMathFish()
    {
        // Числа на рыбах расставляет MathManager через UpdateFishAnswers,
        // нам нужно только создать нужное количество рыб
        for (int i = 0; i < mathFishCount; i++)
        {
            GameObject newFish = SpawnFishAtRandom();
            // FishMath должен быть на префабе — MathManager сам назначит число
        }

        // После спавна говорим MathManager показать первый вопрос
        // (он сам вызовет UpdateFishAnswers и расставит числа)
        if (mathManager != null)
            mathManager.ShowQuestion();
        else
            Debug.LogWarning("FishManager: mathManager не назначен!");
    }

    // --- ОБЩИЙ СПАВН ---
    GameObject SpawnFishAtRandom()
    {
        GameObject prefab = fishPrefabs[Random.Range(0, fishPrefabs.Length)];

        // Сначала создаём рыбу в нулевой точке
        GameObject newFish = Instantiate(prefab, Vector3.zero, Quaternion.identity);

        FishMovement movement = newFish.GetComponent<FishMovement>();
        if (movement != null)
        {
            // Берём границы прямо из FishMovement
            float x = Random.Range(movement.minX, movement.maxX);
            float y = Random.Range(movement.minY, movement.maxY);
            newFish.transform.position = new Vector3(x, y, 0);

            movement.speed = Random.Range(1f, 2.5f);
            movement.movingRight = (Random.value > 0.5f);
        }

        return newFish;
    }

    // --- ОБНОВЛЕНИЕ (для словарного режима) ---
    public void RefreshFishForNewWord(string newWord)
    {
        DestroyAllFish();
        targetWord = newWord;
        SpawnWordFish();
    }

    void DestroyAllFish()
    {
        GameObject[] oldFish = GameObject.FindGameObjectsWithTag("Fish");
        var destroyed = new HashSet<GameObject>();
        foreach (GameObject fish in oldFish)
        {
            GameObject root = fish.transform.root.gameObject;
            if (!destroyed.Contains(root))
            {
                Destroy(root);
                destroyed.Add(root);
            }
        }
    }

    // --- АКУЛА ---
    public void SpawnShark()
    {
        if (activeShark != null) return;

        // Берём границы из любой живой рыбы
        FishMovement anyFish = FindAnyObjectByType<FishMovement>();
        float minX = anyFish != null ? anyFish.minX : -13f;
        float maxX = anyFish != null ? anyFish.maxX : 13f;
        float minY = anyFish != null ? anyFish.minY : -16.5f;
        float maxY = anyFish != null ? anyFish.maxY : -7.5f;

        Vector3 pos = new Vector3(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY),
            0
        );
        activeShark = Instantiate(sharkPrefab, pos, Quaternion.identity);
    }
}