using UnityEngine;

public class HookController : MonoBehaviour
{
    [Header("Настройки движения")]
    public float speed = 5f;
    public float introFallSpeed = 5f;
    public bool canMove = false;
    private bool isIntro = false;
    private bool isReelingIn = false;
    // Потолок для крючка в режиме игры

    [Header("Ссылки")]
    public Transform beachPoint;
    public CameraController camControl;

    public void OnCastAnimationFinished()
    {
        // 1. Находим компонент Animator на этом объекте
        Animator anim = GetComponent<Animator>();

        // 2. Если он есть, выключаем его
        if (anim != null)
        {
            anim.enabled = false;
            Debug.Log("Аниматор выключен. Теперь скрипт может двигать крючок!");
        }

        // 3. Запускаем падение и движение камеры
        isIntro = true;
        camControl.currentState = CameraController.CameraState.FollowingHook;
    }

    void Update()
    {
        /*// СТАРТ ИГРЫ: Нажимаем пробел на берегу
        if (Input.GetKeyDown(KeyCode.Space)) //!isIntro && !canMove && !isReelingIn && 
        {
            StartFishing();
        }*/

        // 1. АВТОМАТИЧЕСКИЙ СПУСК
        if (isIntro)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(transform.position.x, -13f, 0), introFallSpeed * Time.deltaTime);

            if (Mathf.Abs(transform.position.y - (-13f)) < 0.1f)
            {
                isIntro = false;
                canMove = true; // Теперь игрок может управлять
                camControl.currentState = CameraController.CameraState.LockedAtBottom; // Фиксируем камеру!
            }
            return;
        }

        // 2. СВОБОДНАЯ ИГРА (WASD)
        if (canMove)
        {
            float minX = -8.7f;
            float maxX = 8.7f;
            float minY = -17f; // Дно для крючка (может быть глубже, чем камера)
            float maxY = -7.5f;
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");

            Vector3 move = new Vector3(moveX, moveY, 0) * speed * Time.deltaTime;
            transform.position += move;

            // ОГРАНИЧИТЕЛИ: Крючок не выйдет за эти рамки
            float cx = Mathf.Clamp(transform.position.x, minX, maxX);
            float cy = Mathf.Clamp(transform.position.y, minY, maxY);
            transform.position = new Vector3(cx, cy, 0);

            // Если поймали рыбу (условно на пробел)
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Тут должна быть проверка коллайдера рыбы, как в прошлом шаге
                StartReeling();
            }
        }

        // 3. ВОЗВРАТ НАВЕРХ
        if (isReelingIn)
        {
            transform.position = Vector3.MoveTowards(transform.position, beachPoint.position, introFallSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, beachPoint.position) < 0.1f)
            {
                isReelingIn = false;
                camControl.currentState = CameraController.CameraState.AtBeach;
            }
        }
    }

    void StartReeling()
    {
        canMove = false;
        isReelingIn = true;
        camControl.currentState = CameraController.CameraState.FollowingHook; // Камера снова едет за крючком
    }
}

// В месте, где ты готовишься к новому забросу:
//GetComponent<Animator>().enabled = true;

/*using UnityEngine;

public class HookController : MonoBehaviour
{
    public float speed = 5f;
    public float introFallSpeed = 3f; // Скорость падения в начале
    public bool canMove = false;
    public bool isIntro = true;      // Флаг начальной анимации

    [Header("Точки и Границы")]
    public Transform beachPoint;     // Стартовая точка (у рыбака)
    public float bottomY = -12f;     // Глубина, где остановится спуск

    private bool isReelingIn = false;

    void Update()
    {
        // 1. АВТОМАТИЧЕСКИЙ СПУСК В НАЧАЛЕ
        if (isIntro)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(transform.position.x, bottomY, 0), introFallSpeed * Time.deltaTime);

            // Если достигли дна — выключаем интро и даем управление
            if (Mathf.Abs(transform.position.y - bottomY) < 0.1f)
            {
                isIntro = false;
                canMove = true;
            }
            return;
        }

        // 2. ВОЗВРАТ НАВЕРХ (после поимки)
        if (isReelingIn)
        {
            transform.position = Vector3.MoveTowards(transform.position, beachPoint.position, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, beachPoint.position) < 0.1f)
            {
                isReelingIn = false;
                // Здесь можно вызвать событие "Рыба поймана!"
            }
            return;
        }

        // 3. ОБЫЧНОЕ УПРАВЛЕНИЕ WASD (код из прошлых шагов)
        if (canMove)
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");
            transform.position += new Vector3(moveX, moveY, 0) * speed * Time.deltaTime;
            // Не забудь здесь Clamp (ограничение движения), чтобы не уплыть за экран
        }
    }

    // Эту функцию вызываем, когда рыба поймана (через пробел)
    public void StartReeling()
    {
        canMove = false;
        isReelingIn = true;
    }
}*/