using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.TimeZoneInfo;

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

    [Header("Transition Settings")]
    public Animator transitionAnimator;
    public float transitionTime = 1f;

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

        StartCoroutine(LoadLevel("GameScene"));
    }

    public void StartMathMode()
    {
        currentMode = GameMode.Math;
        currentState = GameState.Playing;
        StartCoroutine(LoadLevel("GameScene"));
    }

    IEnumerator LoadLevel(string sceneName)
    {
        SetTransitionAnimation(true);

        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);
        
    }

    public void SetTransitionAnimation(bool isActive)
    {
        transitionAnimator.SetBool("StartAnim", isActive);
    }
}
