using UnityEngine;
using System.Collections;

public class SharkMovement : MonoBehaviour
{
    [Header("Настройки движения")]
    public float speed = 5f; // Акула по умолчанию быстрее рыб
    public bool movingRight = true;
    private bool isStunned = false;

    // Твои границы (как у рыб)
    public float minX = -13f;
    public float maxX = 13f;
    public float minY = -16.5f;
    public float maxY = -7.5f;

    private bool isWaiting = false;
    private SpriteRenderer sprite;

    void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        UpdateFacing();
    }

    void Update()
    {
        if (isWaiting || isStunned) return;

        // Движение акулы
        float direction = movingRight ? 1f : -1f;
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        // Проверка границ и респаун
        if (movingRight && transform.position.x >= maxX)
            StartCoroutine(RespawnShark(minX));
        else if (!movingRight && transform.position.x <= minX)
            StartCoroutine(RespawnShark(maxX));
    }

    IEnumerator RespawnShark(float spawnX)
    {
        isWaiting = true;

        // Прячем акулу (включая все части)
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (var r in renderers) r.enabled = false;

        // Пауза перед новым появлением (акула появляется внезапно)
        yield return new WaitForSeconds(Random.Range(2f, 5f));

        // Новая позиция
        float randomY = Random.Range(minY, maxY);
        transform.position = new Vector3(spawnX, randomY, transform.position.z);

        // Показываем обратно
        foreach (var r in renderers) r.enabled = true;

        isWaiting = false;
    }

    void UpdateFacing()
    {
        if (sprite != null) sprite.flipX = !movingRight;
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
                //HealthManager.Instance.TakeDamage(1); // Уменьшаем здоровье на 1
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
