using UnityEngine;

public class ResolutionHandler : MonoBehaviour
{
    void Awake()
    {
        // Берём нативное разрешение монитора и применяем
        Resolution native = Screen.resolutions[Screen.resolutions.Length - 1];
        Screen.SetResolution(native.width, native.height, true);
    }
}