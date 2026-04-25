using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform beachPoint;
    public Transform hook;
    public float followSpeed = 5f;
    public float minY = -12f;
    public float maxY = 5f;

    public enum CameraState { AtBeach, FollowingHook, LockedAtBottom }
    public CameraState currentState = CameraState.AtBeach;

    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.15f;

    [Header("Затемнение")]
    public DarknessController darknessController;
    public bool darknessTriggered = false; // чтобы не вызывать каждый кадр

    void LateUpdate()
    {
        float targetY = transform.position.y;

        switch (currentState)
        {
            case CameraState.AtBeach:
                targetY = maxY;
                darknessTriggered = false; // сбрасываем при возврате наверх
                break;

            case CameraState.FollowingHook:
                targetY = Mathf.Clamp(hook.position.y, minY, maxY);
                break;

            case CameraState.LockedAtBottom:
                targetY = minY;

                // Включаем затемнение только когда камера приехала вниз
                // и LevelManager говорит что пора (5+ правильных ответов)
                if (!darknessTriggered && darknessController != null)
                {
                    bool shouldBeDark = LevelManager.Instance != null &&
                    LevelManager.Instance.correctAnswersThisLevel >= 3 &&
                    LevelManager.Instance.correctAnswersThisLevel < 5;
                    if (shouldBeDark)
                    {
                        darknessTriggered = true;
                        darknessController.EnableDarkness();
                    }
                }
                break;
        }

        Vector3 targetPos = new Vector3(transform.position.x, targetY, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, followSpeed * Time.deltaTime);
    }
}