using UnityEngine;
using TMPro;

public class WordManager : MonoBehaviour
{
    public string targetWord = "MAMA";
    public TextMeshProUGUI wordDisplay; // Ссылка на наш WordText из UI

    private string collectedWord = "";
    private int currentProgress = 0;

    void Start()
    {
        UpdateUI();
    }

    public bool AddLetter(string letter)
    {
        // Проверяем, совпадает ли буква с текущей нужной в слове
        if (currentProgress < targetWord.Length &&
            letter.Trim().ToUpper() == targetWord[currentProgress].ToString().ToUpper())
        {
            currentProgress++;
            UpdateUI();
            return true;
            Debug.Log("Буква верная! Прогресс: " + currentProgress);
        }
        else
        {
            return false;
            Debug.Log("Не та буква. Нужно: " + targetWord[currentProgress]);
        }
    }

    void UpdateUI()
    {
        string display = "";
        for (int i = 0; i < targetWord.Length; i++)
        {
            if (i < currentProgress) display += targetWord[i] + " ";
            else display += "_ ";
        }
        wordDisplay.text = display;
    }
}