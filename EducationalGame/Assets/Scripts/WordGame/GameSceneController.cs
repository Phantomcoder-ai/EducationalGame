using System.Collections;
using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    public CameraController cameraController;
    public HookController hookController;
    public Animator fishermanAnimator; // Ссылка на аниматор рыбака

    private bool isFishingStarted = false;

    void Update()
    {
        // Проверяем нажатие Пробела и что игра еще не началась
        if (Input.GetKeyDown(KeyCode.Space) && !isFishingStarted)
        {
            StartFishingSequence();
        }
    }

    // Метод, который запускает весь процесс
    public void StartFishingSequence()
    {
        isFishingStarted = true;

        // Вместо fishermanAnimator используем аниматор, который на самом крючке
        // Мы можем достучаться до него через hookController
        Animator hookAnim = hookController.GetComponent<Animator>();

        if (hookAnim != null)
        {
            // Включаем аниматор (на случай, если он был выключен после прошлого раза)
            hookAnim.enabled = true;
            hookAnim.SetTrigger("CastFishingRod");
            Debug.Log("Анимация КРЮЧКА запущена.");
        }
        else
        {
            Debug.LogError("На объекте Hook не найден Animator!");
        }
    }

    // Этот метод можно вызвать позже (например, когда рыба поймана), 
    // чтобы игрок снова мог нажать пробел для нового заброса.
    public void ResetFishingStatus()
    {
        isFishingStarted = false;
    }
}