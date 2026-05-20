using UnityEngine;
using System.Collections;

public class GoldenFish : MonoBehaviour
{
    [Header("Настройки")]
    public float spawnChance = 0.3f;    // шанс появления (30%)
    public float lifetime = 30f;         // сколько секунд живёт на сцене
    public float speedMultiplier = 3.5f;   // быстрее обычных рыб

    private bool caught = false;

    void Start()
    {
        FishMovement movement = GetComponent<FishMovement>();
        if (movement != null)
        {
            movement.speed *= speedMultiplier;
            movement.originalSpeed = movement.speed; // сохраняем чтобы паника не сломала
        }
        StartCoroutine(LifetimeRoutine());
    }

    IEnumerator LifetimeRoutine()
    {
        yield return new WaitForSeconds(lifetime);
        if (!caught)
        {
            Debug.Log("Золотая рыбка уплыла!");
            Destroy(gameObject);
        }
    }

    public void Catch()
    {
        if (caught) return;
        caught = true;
        AudioManager.Instance?.PlayGoldenFish();
        // Восстанавливаем одно сердечко
        if (HealthManager.Instance != null)
            HealthManager.Instance.Heal(1);

        // Бонусные очки
        if (LevelManager.Instance != null)
            LevelManager.Instance.totalScore += 50;

        Debug.Log("Поймал золотую рыбку! +1 сердечко, +50 очков");
        Destroy(gameObject);
    }
}