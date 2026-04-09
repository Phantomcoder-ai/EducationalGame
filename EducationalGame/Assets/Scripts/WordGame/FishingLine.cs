using UnityEngine;

public class FishingLine : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public Transform fishermanTip; // Точка на кончике удочки рыбака
    public Transform hookPoint;      // Точка на самом крючке (можно указать сам Hook)

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // Указываем, что у линии всего 2 точки (начало и конец)
        lineRenderer.positionCount = 2;
    }

    void LateUpdate()
    {
        if (fishermanTip != null && hookPoint != null)
        {
            // Точка 0 — это удочка
            lineRenderer.SetPosition(0, fishermanTip.position);

            // Точка 1 — это крючок
            lineRenderer.SetPosition(1, hookPoint.position);
        }
    }
}