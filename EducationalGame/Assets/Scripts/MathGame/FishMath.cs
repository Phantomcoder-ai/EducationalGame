using UnityEngine;
using TMPro;

public class FishMath : MonoBehaviour
{
    [Header("Данные числа")]
    public int assignedNumber;
    public TextMeshProUGUI textDisplay; // Перетащи сюда Text(TMP) из дочернего Canvas

    // Метод для установки числа (вызывается из MathManager)
    public void SetMathValue(int number)
    {
        assignedNumber = number;
        if (textDisplay != null)
        {
            textDisplay.text = number.ToString();
        }
    }
}