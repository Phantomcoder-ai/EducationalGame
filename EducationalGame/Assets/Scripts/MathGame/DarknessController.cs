using UnityEngine;

public class DarknessController : MonoBehaviour
{
    [Header("Объекты")]
    public GameObject darknessOverlay;
    public Transform hookTransform;

    [Header("Настройки")]
    public float followSpeed = 20f;
    public float fadeInDuration = 2f; // секунд на появление

    private bool isActive = false;
    private bool isFading = false;
    private float fadeTimer = 0f;
    private CanvasGroup canvasGroup; // для плавного появления

    void Awake()
    {
        // Ищем CanvasGroup на overlay (добавь его в инспекторе на DarknessOverlay)
        if (darknessOverlay != null)
            canvasGroup = darknessOverlay.GetComponent<CanvasGroup>();
    }

    void Update()
    {
        // Плавное появление
        if (isFading)
        {
            fadeTimer += Time.deltaTime;
            float alpha = Mathf.Clamp01(fadeTimer / fadeInDuration);

            if (canvasGroup != null)
                canvasGroup.alpha = alpha;

            if (alpha >= 1f)
                isFading = false;
        }

        // Следование за крючком
        if (!isActive || darknessOverlay == null || hookTransform == null) return;

        Vector3 target = new Vector3(
            hookTransform.position.x,
            hookTransform.position.y,
            darknessOverlay.transform.position.z
        );

        darknessOverlay.transform.position = Vector3.Lerp(
            darknessOverlay.transform.position,
            target,
            followSpeed * Time.deltaTime
        );
    }

    public void EnableDarkness()
    {
        if (isActive) return; // уже включено

        isActive = true;
        fadeTimer = 0f;
        isFading = true;

        if (darknessOverlay != null)
        {
            darknessOverlay.SetActive(true);

            // Начинаем с нулевой прозрачности
            if (canvasGroup != null)
                canvasGroup.alpha = 0f;
        }

        Debug.Log("Темнота включена — плавное появление!");
    }

    public void DisableDarkness()
    {
        isActive = false;
        isFading = false;

        if (darknessOverlay != null)
            darknessOverlay.SetActive(false);
    }
}