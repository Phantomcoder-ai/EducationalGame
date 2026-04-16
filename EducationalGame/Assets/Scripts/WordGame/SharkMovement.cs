using UnityEngine;
using System.Collections;

public class SharkMovement : MonoBehaviour
{
    [Header("Настройки движения")]
    public float speed = 5f; // Акула по умолчанию быстрее рыб
    private bool movingRight = true;
    private bool isStunned = false;

    // Твои границы (как у рыб)
    public float minX = -13f;
    public float maxX = 13f;
    public float minY = -16.5f;
    public float maxY = -7.5f;

    private bool isWaiting = false;
    private SpriteRenderer sprite;
    private SpriteRenderer[] renderers;
    void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        renderers = GetComponentsInChildren<SpriteRenderer>();
        SetRandomSideAndDirection();
        UpdateFacing();
    }

    void Update()
    {
        if (isWaiting || isStunned) return;

        // Движение акулы
        float direction = movingRight ? 1f : -1f;
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        // Проверка границ и респаун
        if ((movingRight && transform.position.x >= maxX + 2f) || // Добавили отступ за экран
            (!movingRight && transform.position.x <= minX - 2f))
        {
            StartCoroutine(RespawnShark()); // Теперь просто респаун
        }
    }

    IEnumerator RespawnShark()
    {
        isWaiting = true;

        // Прячем акулу (включая все части)
        foreach (var r in renderers) r.enabled = false;

        // Пауза перед новым появлением (акула появляется внезапно)
        yield return new WaitForSeconds(Random.Range(2f, 5f));

        // ВЫБИРАЕМ НОВУЮ СТОРОНУ И ГЛУБИНУ
        SetRandomSideAndDirection();

        // Показываем обратно
        foreach (var r in renderers) r.enabled = true;

        isWaiting = false;
    }

    void SetRandomSideAndDirection()
    {
        // 1. Выбираем случайную сторону: true - слева, false - справа
        bool spawnFromLeft = Random.Range(0, 2) == 0;

        // 2. Устанавливаем позицию появления за границей экрана
        // Добавляем отступ, чтобы она выплывала плавно
        float spawnX = spawnFromLeft ? minX - 1f : maxX + 1f;
        float randomY = Random.Range(minY, maxY);
        transform.position = new Vector3(spawnX, randomY, transform.position.z);

        // 3. Устанавливаем направление движения:
        // Если появилась слева, плывем вправо, и наоборот
        movingRight = spawnFromLeft;

        // 4. Обновляем визуальный поворот
        UpdateFacing();
    }

    void UpdateFacing()
    {
        if (sprite != null)
        {
            // flipX = true, если плывем ВЛЕВО (false)
            // flipX = false, если плывем ВПРАВО (true)
            sprite.flipX = !movingRight;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hook"))
        {
            Debug.Log("<color=red>Акула ударила крючок!</color>");

            if (CameraShake.Instance != null)
            {
                CameraShake.Instance.Shake(0.3f, 0.4f);
            }
            HookController hook = other.GetComponent<HookController>();
            // 2. Заставляем крючок подняться
            if (hook != null)
            {
                hook.ForceReturn(); // Отпускаем рыбу обратно в воду
            }
            if (HealthManager.Instance != null)
            {
                HealthManager.Instance.TakeDamage(1);
            }

            StartCoroutine(StunShark());
        }
    }
    IEnumerator StunShark()
    {
        isStunned = true; // Останавливаем движение в Update
        
        // Выбираем случайное время паузы (например, от 0.5 до 1.5 секунд)
        float pauseTime = Random.Range(0.5f, 1.5f);
        yield return new WaitForSeconds(pauseTime);

        // Разворачиваемся
        movingRight = !movingRight;
        UpdateFacing();

        isStunned = false; // Снова разрешаем движение
    }

}
