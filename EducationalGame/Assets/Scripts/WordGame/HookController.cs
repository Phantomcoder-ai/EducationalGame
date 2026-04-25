using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HookController : MonoBehaviour
{
    [Header("Настройки движения")]
    public float speed = 5f;
    public float introFallSpeed = 5f;
    public bool canMove = false;
    private bool isIntro = false;
    private bool isReelingIn = false;
     
    public WordManager wordManager;
    public MathManager mathManager; // Добавлено: поддержка MathManager
    private GameObject fishInRange = null; // рыба под крючком (в зоне)
    private GameObject caughtFish = null;   // пойманная рыба (прикреплена к крючку)
    // Потолок для крючка в режиме игры

    [Header("Ссылки")]
    public Transform beachPoint;
    public CameraController camControl;
    public GameSceneController gameSceneController;


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
        UpdateVisualDirection();
        // 1. АВТОМАТИЧЕСКИЙ СПУСК
        if (isIntro)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(transform.position.x, -13f, 0), introFallSpeed * Time.deltaTime);

            if (transform.position.y <= (-13f) + 0.01f)
            {
                isIntro = false;
                canMove = true;
                camControl.currentState = CameraController.CameraState.LockedAtBottom;

                // Возобновляем темноту когда крючок опустился
                DarknessController darkness = FindAnyObjectByType<DarknessController>();
                if (darkness != null) darkness.ResumeDarkness();
                // Запускаем таймер если он активен
                TimerController timer = FindAnyObjectByType<TimerController>();
                if (timer != null && timer.gameObject.activeSelf)
                    timer.StartTimer();
            }
            return;
        }

        // 2. СВОБОДНАЯ ИГРА (WASD)
        if (canMove)
        {
            float minX = -8f;
            float maxX = 8f;
            float minY = -16.5f; // Дно для крючка (может быть глубже, чем камера)
            float maxY = -7f;
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");

            Vector3 move = new Vector3(moveX, moveY, 0) * speed * Time.deltaTime;
            transform.position += move;

            // ОГРАНИЧИТЕЛИ: Крючок не выйдет за эти рамки
            float cx = Mathf.Clamp(transform.position.x, minX, maxX);
            float cy = Mathf.Clamp(transform.position.y, minY, maxY);
            transform.position = new Vector3(cx, cy, 0);

            // Захват рыбы — только по Enter, когда рыба в зоне
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (fishInRange != null && caughtFish == null)
                {
                    AttachFish(fishInRange);
                    StartReeling();
                }
                else
                {
                    Debug.Log("Нет рыбы под крючком.");
                }
            }
        }

        // 3. ВОЗВРАТ НАВЕРХ
        if (isReelingIn)
        {
            transform.position = Vector3.MoveTowards(transform.position, beachPoint.position, introFallSpeed * Time.deltaTime);
            if (camControl.currentState != CameraController.CameraState.FollowingHook)
            {
                camControl.currentState = CameraController.CameraState.FollowingHook;
            }

            if (Vector3.Distance(transform.position, beachPoint.position) < 0.1f)
            {
                isReelingIn = false;
                camControl.currentState = CameraController.CameraState.AtBeach;
                // Останавливаем и сбрасываем таймер
                TimerController timer = FindAnyObjectByType<TimerController>();
                if (timer != null)
                {
                    timer.StopTimer();
                    timer.ResetTimer();
                }
                // Обработка пойманной рыбы: сначала пытаемся распознать тип и передать в соответствующий менеджер
                if (caughtFish != null)
                {
                    // 1. Проверяем, не математическая ли это рыба
                    FishMath mathData = caughtFish.GetComponent<FishMath>();
                    if (mathData != null && mathManager != null)
                    {
                        if (mathManager.CheckAnswer(mathData.assignedNumber)) Destroy(caughtFish);
                        else ReleaseFishBackToWater();
                    }
                    // 2. Иначе проверяем, не буквенная ли это рыба
                    else
                    {
                        FishLetter letterData = caughtFish.GetComponent<FishLetter>();
                        if (letterData != null && wordManager != null)
                        {
                            if (wordManager.AddLetter(letterData.assignedLetter)) Destroy(caughtFish);
                            else ReleaseFishBackToWater();
                        }
                    }
                    caughtFish = null;
                }
                // Включаем аниматор обратно для следующего заброса
                Animator anim = GetComponent<Animator>();
                if (anim != null) anim.enabled = true;
                Debug.Log("Аниматор включен.");

                // Сигналим контроллеру сцены, что можно снова нажать пробел
                if (gameSceneController != null)
                {
                    gameSceneController.ResetFishingStatus();
                }
            }
        }
    }

    void ReleaseFishBackToWater()
    {
        if (caughtFish == null) return;

        // 1. Отцепляем от родителя
        caughtFish.transform.SetParent(null);

        // 2. Включаем физику и ИИ обратно
        Rigidbody2D fishRb = caughtFish.GetComponent<Rigidbody2D>();
        if (fishRb != null)
        {
            fishRb.simulated = true;
            fishRb.linearVelocity = Vector2.zero; // Сбрасываем скорость
        }

        var fm = caughtFish.GetComponent<FishMovement>();
        if (fm != null) fm.enabled = true;

        // 3. Перемещаем в случайное место воды
        float randomY = Random.Range(-15f, -8f);
        caughtFish.transform.position = new Vector3(transform.position.x, randomY, 0);

        caughtFish = null;

        Debug.Log("Буква не та! Рыба возвращена в море.");
    }


    // Фиксируем рыбу в зоне крючка — но не приклеиваем!
    void OnTriggerEnter2D(Collider2D other)
    {
        // Игнорируем все столкновения пока крючок опускается
        if (isIntro) return;

        // Ищем компонент движения рыбы в родителе/самом объекте
        var fishMove = other.GetComponentInParent<FishMovement>();
        if (fishMove != null)
        {
            fishInRange = fishMove.gameObject;
            Debug.Log("Рыба в зоне крючка. Нажми Enter, чтобы поймать.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var fishMove = other.GetComponentInParent<FishMovement>();
        if (fishMove != null && fishInRange == fishMove.gameObject)
        {
            fishInRange = null;
            Debug.Log("Рыба покинула зону крючка.");
        }
    }

    // Приклеиваем рыбу к крючку (вызывается при нажатии Enter)
    private void AttachFish(GameObject fish)
    {
        if (fish == null) return;
        caughtFish = fish;

        // 1. Ищем компоненты (включая дочерние)
        Rigidbody2D fishRb = caughtFish.GetComponent<Rigidbody2D>();
        CircleCollider2D hookCircle = GetComponentInChildren<CircleCollider2D>();
        CircleCollider2D fishCircle = caughtFish.GetComponentInChildren<CircleCollider2D>();

        if (hookCircle == null || fishCircle == null)
        {
            Debug.LogError("Не удалось найти CircleCollider2D на крючке или рыбе!");
            return;
        }

        // 2. Отключаем физику рыбы
        if (fishRb != null) fishRb.simulated = false;

        // 3. ВЫРАВНИВАНИЕ ПО ЦЕНТРАМ В МИРОВЫХ КООРДИНАТАХ
        // Находим, где в мире находится центр круга крючка
        Vector3 hookCenterWorld = hookCircle.transform.TransformPoint(hookCircle.offset);

        // Находим, где в мире находится центр круга рыбы
        Vector3 fishCenterWorld = fishCircle.transform.TransformPoint(fishCircle.offset);

        // Вычисляем вектор смещения между ними
        Vector3 diff = hookCenterWorld - fishCenterWorld;

        // Сдвигаем ВСЮ рыбу на этот вектор (теперь центры коллайдеров совпали в мире)
        caughtFish.transform.position += diff;

        // 4. ПРИВЯЗКА
        // Теперь, когда они стоят идеально, делаем рыбу ребенком
        caughtFish.transform.SetParent(this.transform);

        // Сбрасываем поворот (если нужно, чтобы рыба висела ровно)
        caughtFish.transform.localRotation = Quaternion.identity;

        // Принудительно ставим Z в 0, чтобы рыба не была слишком глубоко или близко
        Vector3 lp = caughtFish.transform.localPosition;
        caughtFish.transform.localPosition = new Vector3(lp.x, lp.y, 0);
        // Выключаем движение рыбы
        FishMovement fm = caughtFish.GetComponent<FishMovement>();
        if (fm == null) fm = caughtFish.GetComponentInChildren<FishMovement>();
        if (fm != null) fm.enabled = false;

        // Сбрасываем флаг зоны (мы уже поймали эту рыбу)
        if (fishInRange == caughtFish) fishInRange = null;

        Debug.Log("Рыба прикреплена к крючку.");
    }

    public void ForceReturn()
    {
        isIntro = false;
        StartReeling();
        // 2. Если на крючке в этот момент была рыба — её нужно отцепить (съела акула)
        // Предположим, рыба становится ребенком крючка при поимке:
        if (transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                // Если это рыба, уничтожаем её или отцепляем
                if (child.CompareTag("Fish"))
                {
                    Destroy(child.gameObject);
                    Debug.Log("Акула заставила бросить рыбу!");
                }
            }
        }

        // 3. Можно добавить визуальный эффект (например, крючок дергается)
        Debug.Log("Крючок принудительно возвращается вверх!");
    }

    void UpdateVisualDirection()
    {
        if (!canMove) return;
        // 1. Получаем ввод игрока (-1 для "влево", 1 для "вправо", 0 если не жмем)
        float moveX = Input.GetAxisRaw("Horizontal");

        // 2. Если кнопка нажата (moveX не равен 0)
        if (moveX != 0)
        {
            // Берем текущий масштаб объекта
            Vector3 newScale = transform.localScale;

            // Устанавливаем масштаб по X: 
            // Берем базовый размер (Mathf.Abs) и умножаем на направление (1 или -1)
            newScale.x = Mathf.Abs(newScale.x) * Mathf.Sign(moveX);

            // Применяем новый масштаб к объекту
            transform.localScale = newScale;
        }
    }

    void StartReeling()
    {
        canMove = false;
        isReelingIn = true;
        camControl.currentState = CameraController.CameraState.FollowingHook; // Камера снова едет за крючком

        // Скрываем темноту пока крючок поднимается
        DarknessController darkness = FindAnyObjectByType<DarknessController>();
        if (darkness != null) darkness.PauseDarkness();
    }
}