using System.Collections;
using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    public CameraController cameraController;
    public HookController hookController;

    public float introTime = 2f;

    void Start()
    {
        StartCoroutine(Intro());
    }

    IEnumerator Intro()
    {
        // 1. Ждем немного на берегу
        yield return new WaitForSeconds(5f);

        // 2. Включаем слежение за крючком
        cameraController.currentState = CameraController.CameraState.FollowingHook;

        // 3. Даем управление игроку
        hookController.canMove = true;
    }

    /*IEnumerator Intro()
    {;

        // 🎥 движение камеры
        cameraController.move = true;

        yield return new WaitForSeconds(introTime);

        // 🛑 остановка
        cameraController.move = false;

        // 🎮 включаем управление
        hookController.canMove = true;
    }*/
}