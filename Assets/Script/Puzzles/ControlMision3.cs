using UnityEngine;

public class ControlMision3 : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject puerta;
    public GameObject CartelIncorrecto;
    public GameObject CartelCorrecto;
    public GameObject TrofeoMision3;
    public AudioSource audioSource;
    public AudioClip sonidoCorrecto;
    public AudioClip sonidoIncorrecto;
    [Header("Orden correcto de bancos (por número)")]
    public int[] ordenCorrecto = { 3, 1, 2, 2, 1 };

    private int indiceActual = 0;

       public void RegistrarBanco(int idBanco)
    {
        Debug.Log("Pisaste banco " + idBanco);

        // Verifica si el banco tocado es el correcto
        if (ordenCorrecto[indiceActual] == idBanco)
        {
            indiceActual++;
            Debug.Log("Correcto! Paso " + indiceActual + "/" + ordenCorrecto.Length);
            audioSource.PlayOneShot(sonidoCorrecto);
            if (indiceActual >= ordenCorrecto.Length)
            {
                PuzzleCompletado();
            }
        }
        else
        {
            ReiniciarPuzzle();
        }
    }

    private void ReiniciarPuzzle()
    {
        
        Debug.Log("Banco incorrecto. Reiniciando...");
        indiceActual = 0;
        CartelIncorrecto.SetActive(true);
        //hagamos que el cartel dure un segundo y luego desaparezca
        Invoke("OcultarCartelIncorrecto", 1f);
        audioSource.PlayOneShot(sonidoIncorrecto);

    }

    private void PuzzleCompletado()
    {
        Debug.Log("¡Puzzle completado!");
        if (puerta != null)
        {
            puerta.SetActive(false); // por ejemplo, la puerta desaparece
            TrofeoMision3.SetActive(true);
            CartelCorrecto.SetActive(true);
            Invoke("OcultarCartelCorrecto", 2f);
            audioSource.PlayOneShot(sonidoCorrecto);

        }
    }
    private void OcultarCartelIncorrecto()
    {
        CartelIncorrecto.SetActive(false);
        
    }
    private void OcultarCartelCorrecto()
    {
        CartelCorrecto.SetActive(false);
        
    }
}
