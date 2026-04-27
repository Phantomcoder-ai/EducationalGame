using UnityEngine;
using System.Collections;

public class GoldenFish : MonoBehaviour
{
    [Header("Настройки")]
    public float spawnChance = 0.3f;    // шанс появления (30%)
    public float lifetime = 10f;         // сколько секунд живёт на сцене
    public float speedMultiplier = 2f;   // быстрее обычных рыб

    private bool caught = false;

    void Start()
    {
        FishMovement movement = GetComponent<FishMovement>();
        if (movement != null)
            movement.speed *= speedMultiplier;

        // Автоматически исчезает через lifetime секунд
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