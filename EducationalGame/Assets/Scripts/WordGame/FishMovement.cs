using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

public class FishMovement : MonoBehaviour
{
    [Header("Настройки движения")]
    public float speed = 2f;
    public float minX = -13f;
    public float maxX = 13f;
    public float minY = -16.5f;
    public float maxY = -7.5f;

    public bool movingRight = true;
    private bool isWaiting = false;

    public SpriteRenderer fishBodySprite; // Ссылка на спрайт ТЕЛА рыбы
    public Transform leafTransform;      // Ссылка на ТРАНСФОРМ листка

    void Start()
    {
        
        // Случайно выбираем начальное направление
        movingRight = Random.value > 0.5f;
        UpdateFacing();
    }

    void Update()
    {
        if (isWaiting) return;
        // Движение родительского объекта
        float direction = movingRight ? 1f : -1f;
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        // Проверка выхода за границы
        if (movingRight && transform.position.x >= maxX)
        {
            // Уплыла вправо -> респаун слева
            StartCoroutine(RespawnFish(minX));
        }
        else if (!movingRight && transform.position.x <= minX)
        {
            // Уплыла влево -> респаун справа
            StartCoroutine(RespawnFish(maxX));
        }
    }

    IEnumerator RespawnFish(float spawnX)
    {
        isWaiting = true;
        // Сначала спрятать рыбу (можно отключить спрайт или переместить вниз)
        if (fishBodySprite != null)
            fishBodySprite.enabled = false;

        SpriteRenderer[] allSprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (var s in allSprites) s.enabled = false;

        // Подождать пару секунд
        float delay = Random.Range(1.0f, 4.0f);
        yield return new WaitForSeconds(delay);

        // Появиться на другой стороне
        float randomY = Random.Range(minY, maxY);
        transform.position = new Vector3(spawnX, randomY, transform.position.z);

        // Сменить направление
        UpdateFacing();

        // 5. Показываем рыбу обратно
        foreach (var s in allSprites) s.enabled = true;
        isWaiting = false;
    }

    void UpdateFacing()
    {
        // Отражаем спрайт рыбы
        if (fishBodySprite != null)
            fishBodySprite.flipX = !movingRight;

        // "Перебрасываем" листок на другую сторону хвоста
        if (leafTransform != null)
        {
            Vector3 leafPos = leafTransform.localPosition;
            // Если плывем вправо, листок должен быть слева от центра (сзади)
            // Если плывем влево, листок должен быть справа от центра (сзади)
            float xOffset = movingRight ? -1.2f : 2.2f; // Настрой это число под свою рыбу
            leafTransform.localPosition = new Vector3(xOffset, leafPos.y, leafPos.z);
        }
    }
}