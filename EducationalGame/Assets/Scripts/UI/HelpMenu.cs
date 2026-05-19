using UnityEngine;
using UnityEngine.EventSystems;

public class HelpMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject helpPanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (helpPanel != null && helpPanel.activeSelf)
                CloseHelp();
        }
    }

    void Start()
    {
        helpPanel.SetActive(false);
    }

    public void OpenHelp()
    {
        helpPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void CloseHelp()
    {
        helpPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }
}