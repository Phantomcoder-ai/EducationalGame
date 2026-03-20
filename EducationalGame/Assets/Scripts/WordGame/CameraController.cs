using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform beachPoint;
    public Transform hook;
    public float followSpeed = 4f;

    public float minY = -12f; // Координата Y "дна"
    public float maxY = 0f;   // Координата Y "берега"

    public enum CameraState { AtBeach, FollowingHook, LockedAtBottom }
    public CameraState currentState = CameraState.AtBeach;

    void LateUpdate()
    {
        Vector3 targetPos = transform.position;

        switch (currentState)
        {
            case CameraState.AtBeach:
                targetPos = new Vector3(transform.position.x, maxY, transform.position.z);
                break;

            case CameraState.FollowingHook:
                // Следим за крючком, но не выходим за границы minY и maxY
                float clampedY = Mathf.Clamp(hook.position.y, minY, maxY);
                targetPos = new Vector3(transform.position.x, clampedY, transform.position.z);

                // Если крючок достиг дна, фиксируем камеру
                if (hook.position.y <= minY)
                {
                    currentState = CameraState.LockedAtBottom;
                }
                break;

            case CameraState.LockedAtBottom:
                // Камера просто стоит на месте внизу
                targetPos = new Vector3(transform.position.x, minY, transform.position.z);
                break;
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
    }
}

/*using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform beachPoint; // Точка берега (верх)
    public Transform hook;       // Крючок
    public float followSpeed = 3f;

    [Header("Границы камеры")]
    public float minY = -12f;    // Самая нижняя точка, куда может упасть камера (дно)
    public float maxY = 5f;      // Самая верхняя точка (берег)

    public enum CameraState { AtBeach, FollowingHook }
    public CameraState currentState = CameraState.AtBeach;

    void LateUpdate()
    {
        Vector3 targetPos;

        if (currentState == CameraState.FollowingHook)
        {
            // Камера пытается следовать за Y-координатой крючка
            float targetY = hook.position.y;

            // ГЛАВНАЯ МАГИЯ: Ограничиваем targetY, чтобы камера не выходила за пределы
            targetY = Mathf.Clamp(targetY, minY, maxY);

            targetPos = new Vector3(transform.position.x, targetY, transform.position.z);
        }
        else
        {
            // Состояние покоя: приклеена к верхней точке (берегу)
            targetPos = new Vector3(transform.position.x, maxY, transform.position.z);
        }

        // Плавное движение к вычисленной точке
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
    }
}*/