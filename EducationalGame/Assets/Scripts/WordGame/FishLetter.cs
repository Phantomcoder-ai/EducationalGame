using UnityEngine;
using TMPro;

public class FishLetter : MonoBehaviour
{
    public TextMeshProUGUI tmpText; // Ссылка на текст внутри листка
    public string assignedLetter;    // Какая буква назначена этой рыбе

    public void SetupLetter(string letter)
    {
        assignedLetter = letter;
        if (tmpText != null)
        {
            tmpText.text = letter;
        }
    }
}