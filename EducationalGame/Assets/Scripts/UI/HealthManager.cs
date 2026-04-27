using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance;

    [Header("Настройки")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("UI элементы")]
    public List<Image> heartImages; // Сюда перетащи 3 иконки сердец
    public Sprite fullHeart;
    public Sprite emptyHeart;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();

        if (currentHealth <= 0)
        {
            TriggerGameOver();
        }
    }

    void UpdateUI()
    {
        for (int i = 0; i < heartImages.Count; i++)
        {
            heartImages[i].sprite = (i < currentHealth) ? fullHeart : emptyHeart;
        }
    }

    public void TriggerGameOver()
    {
        GameSessionData.isVictory = false; // добавь эту строку
        GameSessionData.lastSceneName = SceneManager.GetActiveScene().name;
        Time.timeScale = 1f;
        SceneManager.LoadScene("ResultScene");
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();
        Debug.Log("Восстановлено здоровье: " + currentHealth);
    }
}