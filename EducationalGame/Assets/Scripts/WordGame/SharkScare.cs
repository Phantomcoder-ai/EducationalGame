using UnityEngine;

public class SharkScare : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Fish"))
        {
            // Получаем ссылку на FishMovement и увеличиваем скорость
            FishMovement fish = other.GetComponentInParent<FishMovement>();
            if (fish != null)
            {
                fish.SetPanic(true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Fish"))
        {
            // Получаем ссылку на FishMovement и возвращаем скорость к нормальной
            FishMovement fish = other.GetComponentInParent<FishMovement>();
            if (fish != null)
            {
                fish.SetPanic(false);
            }
        }
    }

}
