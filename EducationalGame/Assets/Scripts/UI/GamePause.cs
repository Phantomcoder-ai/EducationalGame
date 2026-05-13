using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GamePause : MonoBehaviour
{
    [Header("Панели")]
    public GameObject menuPanel;
    public GameObject settingsPanel;

    [Header("Слайдеры")]
    public Slider musicSlider;
    public Slider sfxSlider;

    private bool isPaused = false;

    void Start()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        // Загружаем сохранённые значения
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
                CloseSettings(); // Escape закрывает настройки обратно в паузу
            else
                ResumeGame();
        }
    }

    public void PauseGame()
    {
        menuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
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
        // Прячем кнопки, показываем настройки
        foreach (Transform child in menuPanel.transform)
        {
            if (child.gameObject != settingsPanel)
                child.gameObject.SetActive(false);
        }
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        // Сохраняем и возвращаемся к кнопкам
        SaveSettings();
        settingsPanel.SetActive(false);

        foreach (Transform child in menuPanel.transform)
            child.gameObject.SetActive(true);
    }

    void OnMusicChanged(float value)
    {
        AudioManager.Instance?.SetMusicVolume(value);
    }

    void OnSFXChanged(float value)
    {
        AudioManager.Instance?.SetSFXVolume(value);
    }

    void SaveSettings()
    {
        if (musicSlider != null)
            PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        if (sfxSlider != null)
            PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
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