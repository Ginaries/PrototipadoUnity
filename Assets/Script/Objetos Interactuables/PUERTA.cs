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
        Quaternion rotacionInicial = transform.rotation;
        Quaternion rotacionFinal = rotacionInicial * Quaternion.Euler(0, gradosAbrir, 0);
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracionRotacion)
        {
            transform.rotation = Quaternion.Slerp(rotacionInicial, rotacionFinal, tiempoTranscurrido / duracionRotacion);
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        transform.rotation = rotacionFinal;
        puertaAbierta = true;
        rotando = false;

        StartCoroutine(MostrarCartel());
    }
    
    private IEnumerator MostrarCartel()
    {
        cartelPuertaAbierta.SetActive(true);
        yield return new WaitForSeconds(duracionCartel);
        cartelPuertaAbierta.SetActive(false);
    }
}
