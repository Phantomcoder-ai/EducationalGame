using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public void RestartGame()
    {
        // Вместо конкретного имени сцены используем наше хранилище
        string sceneToLoad = GameSessionData.lastSceneName;

        Debug.Log("Возвращаемся на: " + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);
    }

    public void BackToMainMenu()
    {
        // Загружаем стартовую сцену (если она есть) или просто выходим
        SceneManager.LoadScene("MainMenu");
    }
}