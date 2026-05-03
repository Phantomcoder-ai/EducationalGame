using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject settingsPanel;
    public TMP_Dropdown resolutionDropdown;
    public TextMeshProUGUI fullscreenButtonText;

    private Resolution[] resolutions;
    private int selectedResolutionIndex;
    private bool selectedFullscreen;

    void Start()
    {
        settingsPanel.SetActive(false);
        LoadResolutions();
        LoadSettings();
    }

    void LoadResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        HashSet<string> added = new HashSet<string>();
        int currentIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width} x {resolutions[i].height}";
            if (added.Contains(option)) continue;
            added.Add(option);
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
                currentIndex = options.Count - 1;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionIndex", currentIndex);
        resolutionDropdown.RefreshShownValue();

        selectedResolutionIndex = resolutionDropdown.value;
    }

    void LoadSettings()
    {
        selectedFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        Screen.fullScreen = selectedFullscreen;

        if (fullscreenButtonText != null)
            fullscreenButtonText.text = selectedFullscreen ? "OKNO" : "PEŁNY EKRAN";
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    // Просто запоминаем выбор — не применяем сразу
    public void OnResolutionChanged(int index)
    {
        selectedResolutionIndex = index;
    }

    public void ToggleFullscreen()
    {
        selectedFullscreen = !selectedFullscreen;
        if (fullscreenButtonText != null)
            fullscreenButtonText.text = selectedFullscreen ? "OKNO" : "PEŁNY EKRAN";
    }

    // Применяем всё сразу по кнопке
    public void ApplySettings()
    {
        // Применяем разрешение
        string selected = resolutionDropdown.options[selectedResolutionIndex].text;
        string[] parts = selected.Split('x');
        int width = int.Parse(parts[0].Trim());
        int height = int.Parse(parts[1].Trim());

        Screen.SetResolution(width, height, selectedFullscreen);

        // Сохраняем
        PlayerPrefs.SetInt("ResolutionIndex", selectedResolutionIndex);
        PlayerPrefs.SetInt("Fullscreen", selectedFullscreen ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log($"Применено: {width}x{height}, fullscreen={selectedFullscreen}");
    }
}