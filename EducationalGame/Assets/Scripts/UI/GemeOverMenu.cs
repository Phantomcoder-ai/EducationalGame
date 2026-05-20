using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverMenu : MonoBehaviour
{
    [Header("Панели результата")]
    public GameObject defeatPanel;   // GameOverText — надпись GAME OVER
    public GameObject victoryPanel;  // YOU WIN — надпись победы

    [Header("Общие элементы")]
    public GameObject playAgainText; // PlayAgainText
    public GameObject heartsPanel;   // объект с сердечками (Heart, Heart(1), Heart(2))

    [Header("Очки и уровень (опционально)")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;

    void Start()
    {
        AudioManager.Instance?.PlayMusic(AudioManager.Instance.resultMusic);

        if (GameSessionData.isVictory)
        {
            // Победа
            if (defeatPanel != null) defeatPanel.SetActive(false);
            if (victoryPanel != null) victoryPanel.SetActive(true);
            if (heartsPanel != null) heartsPanel.SetActive(false); // при победе сердца не нужны
            AudioManager.Instance?.PlayCorrect();
        }
        else
        {
            // Поражение
            if (defeatPanel != null) defeatPanel.SetActive(true);
            if (victoryPanel != null) victoryPanel.SetActive(false);
            if (heartsPanel != null) heartsPanel.SetActive(true);
            AudioManager.Instance?.PlayGameOver();
        }

        // Показываем очки если есть TextMeshPro
        if (scoreText != null)
            scoreText.text = "Wynik: " + GameSessionData.score;

        if (levelText != null)
            levelText.text = "Combo: x" + GameSessionData.combo;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(GameSessionData.lastSceneName);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}