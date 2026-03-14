using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Menu,
    Playing,
    Paused,
    Win,
    GameOver
}
public enum GameMode
{
    Words,
    Math
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState currentState;
    public GameMode currentMode;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentState = GameState.Menu;
        Debug.Log("Game started in state: " + currentState);
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;

        Debug.Log("Game State: " + currentState);
    }
    public void StartWordMode()
    {
        currentMode = GameMode.Words;
        currentState = GameState.Playing;

        SceneManager.LoadScene("GameScene");
    }

    public void StartMathMode()
    {
        currentMode = GameMode.Math;
        currentState = GameState.Playing;
        SceneManager.LoadScene("GameScene");
    }
}
