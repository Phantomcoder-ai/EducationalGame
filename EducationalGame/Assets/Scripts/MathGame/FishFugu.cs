using UnityEngine;

public class FishFugu : MonoBehaviour
{
    [Header("Ёффекты")]
    public GameObject explosionEffect; // префаб взрыва (опционально)

    private bool exploded = false;

    public void Explode()
    {
        if (exploded) return;
        exploded = true;

        // ћинус жизнь
        if (HealthManager.Instance != null)
            HealthManager.Instance.TakeDamage(1);

        // —брос комбо
        if (LevelManager.Instance != null)
            LevelManager.Instance.OnWrongAnswer();

        // Ёффект взрыва
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Debug.Log("‘угу взорвалась!");
        Destroy(gameObject);
    }
}