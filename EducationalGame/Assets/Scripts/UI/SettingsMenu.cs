using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject settingsPanel;

    [Header("Слайдеры")]
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        settingsPanel.SetActive(false);

        // Загружаем сохранённые значения (по умолчанию 1.0)
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // Устанавливаем слайдеры
        if (musicSlider != null)
        {
            musicSlider.value = savedMusic;
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = savedSFX;
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        // Применяем к AudioManager
        AudioManager.Instance?.SetMusicVolume(savedMusic);
        AudioManager.Instance?.SetSFXVolume(savedSFX);
    }

    void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance?.SetMusicVolume(value);
    }

    void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance?.SetSFXVolume(value);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void ApplySettings()
    {
        // Сохраняем значения
        if (musicSlider != null)
            PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);

        if (sfxSlider != null)
            PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);

        PlayerPrefs.Save();

        Debug.Log("Настройки сохранены!");
        settingsPanel.SetActive(false);
    }
}