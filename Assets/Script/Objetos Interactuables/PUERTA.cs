using UnityEngine;
using System.Collections;

public class PUERTA : MonoBehaviour
{
    [Header("Estado")]
    public bool tieneLlave = false;
    public bool puertaAbierta = false;

    [Header("Animación")]
    public float gradosAbrir = 90f;
    public float duracionRotacion = 0.6f; // segundos
    private bool rotando = false;

    [Header("UI Cartel")]
    public GameObject cartelPuertaAbierta; // 👈 arrastrá el PNG acá en el inspector
    public float duracionCartel = 2f; // segundos que dura en pantalla

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

    private IEnumerator RotarPuerta()
    {
        rotando = true;

        Quaternion inicio = transform.rotation;
        Quaternion destino = Quaternion.Euler(0f, transform.rotation.eulerAngles.y + gradosAbrir, 0f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duracionRotacion;
            transform.rotation = Quaternion.Slerp(inicio, destino, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        transform.rotation = destino;
        puertaAbierta = true;
        rotando = false;

        // 👇 mostrar cartel cuando termina de abrir
        if (cartelPuertaAbierta != null)
        {
            StartCoroutine(MostrarCartel());
        }
    }

    private IEnumerator MostrarCartel()
    {
        cartelPuertaAbierta.SetActive(true);
        yield return new WaitForSeconds(duracionCartel);
        cartelPuertaAbierta.SetActive(false);
    }
}
