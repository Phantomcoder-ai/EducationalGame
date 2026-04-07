using UnityEngine;

public class FishManager : MonoBehaviour
{
    public GameObject fishPrefab; // Ссылка на префаб Fish_Complete
    public string targetWord = "МАМА";

    [Header("Зона появления")]
    public float spawnMinX = -6f;
    public float spawnMaxX = 6f;
    public float spawnMinY = -15f;
    public float spawnMaxY = -9f;

    void Start()
    {
        SpawnFishForWord();
    }

    void SpawnFishForWord()
    {
        for (int i = 0; i < targetWord.Length; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(spawnMinX, spawnMaxX),
                Random.Range(spawnMinY, spawnMaxY),
                0
            );

            GameObject newFish = Instantiate(fishPrefab, randomPos, Quaternion.identity);

            // Используем новый скрипт для установки буквы
            FishLetter fishLetterScript = newFish.GetComponent<FishLetter>();
            if (fishLetterScript != null)
            {
                fishLetterScript.SetupLetter(targetWord[i].ToString());
            }

            // Случайная скорость
            FishMovement movement = newFish.GetComponent<FishMovement>();
            if (movement != null)
            {
                movement.speed = Random.Range(1.5f, 3.5f);
            }
        }
    }
}