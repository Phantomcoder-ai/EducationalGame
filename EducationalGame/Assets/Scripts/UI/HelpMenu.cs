using UnityEngine;

public class HelpMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject helpPanel;

    void Start()
    {
        helpPanel.SetActive(false);
    }

    public void OpenHelp()
    {
        helpPanel.SetActive(true);
    }

    public void CloseHelp()
    {
        helpPanel.SetActive(false);
    }
}