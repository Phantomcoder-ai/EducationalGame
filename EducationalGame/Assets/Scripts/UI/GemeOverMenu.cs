using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverMenu : MonoBehaviour
{
    public TextMeshProUGUI resultText; // перетащи текст "GAME OVER" сюда

    void Start()
    {
        if (resultText != null)
        {
            resultText.text = GameSessionData.isVictory ? "YOU WIN!" : "GAME OVER";
            resultText.color = GameSessionData.isVictory
                ? new Color(1f, 0.85f, 0f)  // золотой для победы
                : new Color(0f, 0.5f, 1f);  // синий для поражения (как у тебя сейчас)
        }
    }

    public void RestartGame()
    {
        string sceneToLoad = GameSessionData.lastSceneName;
        SceneManager.LoadScene(sceneToLoad);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}