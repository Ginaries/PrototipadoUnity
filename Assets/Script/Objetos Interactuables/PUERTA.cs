using UnityEngine;

public class PUERTA : MonoBehaviour
{
    [Header("Estado")]
    public bool tieneLlave = false;
    public bool puertaAbierta = false;

    [Header("Animación")]
    public float gradosAbrir = 90f;
    public float duracionRotacion = 0.6f; // segundos
    private bool rotando = false;

    public void AbrirPuerta()
    {
        if (!tieneLlave || puertaAbierta || rotando) return;
        StartCoroutine(RotarPuerta());
    }

    public void TieneLlave()
    {
        tieneLlave = true;
        AbrirPuerta();
    }

    private System.Collections.IEnumerator RotarPuerta()
    {
        rotando = true;

        Quaternion inicio = transform.rotation;
        Quaternion destino = Quaternion.Euler(0f, transform.rotation.eulerAngles.y + gradosAbrir, 0f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duracionRotacion;
            transform.rotation = Quaternion.Slerp(inicio, destino, Mathf.SmoothStep(0f, 1f, t));
            yield return null; // cede el frame
        }

        transform.rotation = destino;
        puertaAbierta = true;
        rotando = false;
    }
}
