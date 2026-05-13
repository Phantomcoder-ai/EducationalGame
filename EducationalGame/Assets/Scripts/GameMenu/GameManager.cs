using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.TimeZoneInfo;

public enum GameState
{
    Menu,
    WordPlaying,
    MathPlaying,
    GameOver,
}


public class GameManager : MonoBehaviour
{
    //public static GameManager Instance;
    public GameState currentState;

    [Header("Transition Settings")]
    public float transitionTime = 1f;


    void Start()
    {
        currentState = GameState.Menu;
        Debug.Log("Game started in state: " + currentState);
        AudioManager.Instance?.PlayMusic(AudioManager.Instance.menuMusic);
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
        Debug.Log("Game State: " + currentState);
    }

    public void StartWordMode()
    {
        currentState = GameState.WordPlaying;
        StartCoroutine(LoadLevel("WordPlaying"));
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    public void StartMathMode()
    {
        currentState = GameState.MathPlaying;
        StartCoroutine(LoadLevel("MathPlaying"));
    }

    IEnumerator LoadLevel(string sceneName)
    {
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);
    }

}
