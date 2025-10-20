using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform objetivo; // Asigná el jugador en el Inspector
    public Vector3 offset = new Vector3(0, 5, -6);
    public float suavizado = 5f;

    void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 posicionDeseada = objetivo.position + offset;
        transform.position = Vector3.Lerp(transform.position, posicionDeseada, Time.deltaTime * suavizado);

        // Que la cámara mire siempre al jugador
        transform.LookAt(objetivo);
    }
}
