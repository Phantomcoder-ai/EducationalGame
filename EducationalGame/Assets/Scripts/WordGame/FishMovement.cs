using UnityEngine;

public class FishMovement : MonoBehaviour
{
    [Header("Настройки движения")]
    public float speed = 2f;
    public float leftLimit = -8f;
    public float rightLimit = 8f;

    private bool movingRight = true;
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
        // Движение родительского объекта
        if (movingRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            if (transform.position.x >= rightLimit)
            {
                movingRight = false;
                UpdateFacing();
            }
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
            if (transform.position.x <= leftLimit)
            {
                movingRight = true;
                UpdateFacing();
            }
        }
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
            float xOffset = movingRight ? -1.2f : 1.2f; // Настрой это число под свою рыбу
            leafTransform.localPosition = new Vector3(xOffset, leafPos.y, leafPos.z);
        }
    }
}