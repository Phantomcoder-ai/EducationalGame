using UnityEngine;

public class FishManager : MonoBehaviour
{
    public GameObject[] fishPrefabs; // Ссылка на префабы Fish_Complete
    public string targetWord = "МАМА";


    [Header("Настройки сложности")]
    public int extraFishCount = 10; // Сколько лишних рыб добавить
    public string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";


    [Header("Зона появления")]
    public float spawnMinX = -6f;
    public float spawnMaxX = 6f;
    public float spawnMinY = -15f;
    public float spawnMaxY = -9f;

    void Start()
    {
        SpawnAllFish();
    }

    void SpawnAllFish()
    {
        // 1. Создаем обязательные рыбы для слова
        for (int i = 0; i < targetWord.Length; i++)
        {
            CreateFish(targetWord[i].ToString());
        }

        // 2. Создаем случайные рыбы ("обманки")
        for (int i = 0; i < extraFishCount; i++)
        {
            char randomChar = alphabet[Random.Range(0, alphabet.Length)];
            CreateFish(randomChar.ToString());
        }
    }

    void CreateFish(string letter)
    {
        Vector3 randomPos = new Vector3(
            Random.Range(spawnMinX, spawnMaxX),
            Random.Range(spawnMinY, spawnMaxY),
            0
        );
        int randomPrefabIndex = Random.Range(0, fishPrefabs.Length);
        GameObject fishPrefab = fishPrefabs[randomPrefabIndex];
        GameObject newFish = Instantiate(fishPrefab, randomPos, Quaternion.identity);

        // Устанавливаем букву
        FishLetter fishLetterScript = newFish.GetComponentInChildren<FishLetter>();
        if (fishLetterScript != null)
        {
            fishLetterScript.SetupLetter(letter);
        }

        // Устанавливаем случайную скорость и направление
        FishMovement movement = newFish.GetComponent<FishMovement>();
        if (movement != null)
        {
            movement.speed = Random.Range(1f, 2.5f);
            // Чтобы рыбы не плыли все в одну сторону в начале
            movement.movingRight = (Random.value > 0.5f);
        }
    }

    public void RefreshFishForNewWord(string newWord)
    {
        GameObject[] oldFish = GameObject.FindGameObjectsWithTag("Fish");
        foreach (GameObject fish in oldFish)
        {
            if(fish.transform.parent == null) Destroy(fish);
        }

        targetWord = newWord;

        SpawnAllFish();
    }

}