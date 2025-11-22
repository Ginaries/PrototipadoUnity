using UnityEngine;

public class GestorPuzzle4 : MonoBehaviour
{
    public GameObject puertaFinal;
    public AudioSource audioSource;
    public AudioClip clipPuzzleCompletado;
    public GameObject CartelPuertaabierta;
    public GameObject trofeoPuzzle4;

    private bool placa1pisada = false;
    private bool placa2pisada = false;
    private bool trabajorealizado = false;

    void Update()
    {
        if (!trabajorealizado && placa1pisada && placa2pisada)
        {
            Debug.Log("Puzzle 4 completado");
            FindAnyObjectByType<MetricasJuego>().CompletarMision("Puzzle 4 - Placas Completado");
            audioSource.PlayOneShot(clipPuzzleCompletado);
            trabajorealizado = true;
            puertaFinal.SetActive(false);
            CartelPuertaabierta.SetActive(true);
            trofeoPuzzle4.SetActive(true);
            Invoke(nameof(DesactivarCartel), 3f);
        }
    }

    public void Placa1Pisada(bool valor) => placa1pisada = valor;
    public void Placa2Pisada(bool valor) => placa2pisada = valor;

    public void DesactivarCartel()
    {
        CartelPuertaabierta.SetActive(false);
    }
}
