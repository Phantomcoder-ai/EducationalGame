using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    // Сделаем скрипт доступным из любого места (Синглтон)
    public static CameraShake Instance;

    private Vector3 originalPos;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        originalPos = transform.localPosition;
    }

    // Основной метод для вызова тряски
    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(DoShake(duration, magnitude));
    }

    IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Генерируем случайное смещение
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null; // Ждем до следующего кадра
        }

        // Возвращаем камеру в исходное положение
        transform.localPosition = originalPos;
    }
}