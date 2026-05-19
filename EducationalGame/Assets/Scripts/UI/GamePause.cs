using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePause : MonoBehaviour
{
    [Header("Панели")]
    public GameObject menuPanel;
    public GameObject settingsPanel;

    [Header("Слайдеры")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Информация об уровне")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI scoreText;

    private bool isPaused = false;

    void Start()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (musicSlider != null)
        {
            musicSlider.value = savedMusic;
            musicSlider.onValueChanged.AddListener(OnMusicChanged);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = savedSFX;
            sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                PauseGame();
            else if (settingsPanel != null && settingsPanel.activeSelf)
                CloseSettings();
            else
                ResumeGame();
        }
    }

    public void PauseGame()
    {
        menuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        // Обновляем информацию при открытии паузы
        UpdateStatsUI();
    }

    void UpdateStatsUI()
    {
        if (LevelManager.Instance == null) return;

        if (levelText != null)
            levelText.text = "Poziom: " + LevelManager.Instance.currentLevel;

        if (progressText != null)
        {
            int current = LevelManager.Instance.correctAnswersThisLevel;
            int needed = LevelManager.Instance.gameMode == LevelManager.Mode.Math
                ? LevelManager.Instance.correctAnswersPerLevelMath
                : LevelManager.Instance.correctAnswersPerLevelWord;
            progressText.text = "Postęp: " + current + " / " + needed;
        }

        if (scoreText != null)
            scoreText.text = "Wynik: " + LevelManager.Instance.totalScore;
    }

    public void ResumeGame()
    {
        menuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void OpenSettings()
    {
        foreach (Transform child in menuPanel.transform)
        {
            if (child.gameObject != settingsPanel)
                child.gameObject.SetActive(false);
        }
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        SaveSettings();
        settingsPanel.SetActive(false);
        foreach (Transform child in menuPanel.transform)
            child.gameObject.SetActive(true);
    }

    void OnMusicChanged(float value) => AudioManager.Instance?.SetMusicVolume(value);
    void OnSFXChanged(float value) => AudioManager.Instance?.SetSFXVolume(value);

    void SaveSettings()
    {
        if (musicSlider != null) PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        if (sfxSlider != null) PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        PlayerPrefs.Save();
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToMain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}