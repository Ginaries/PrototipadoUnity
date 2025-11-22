using System;
using System.Collections.Generic;
using UnityEngine;

public class LockersCOLORS : MonoBehaviour
{
    [Header("Lockers de colores (opcional, para referencias visuales)")]
    public GameObject lockerVioleta;
    public GameObject lockerAmarillo;
    public GameObject lockerLila;
    public GameObject lockerVerde;
    public GameObject lockerBlanco;
    public GameObject CartelIncorrecto;
    public GameObject CartelCorrecto;
    public GameObject Trofeo;
    public AudioSource audioSource;
    public AudioClip sonidoCorrecto;
    public AudioClip sonidoIncorrecto;

    [Header("Puertas que se abren")]
    public GameObject puerta1;

    [Header("Secuencia correcta (orden de colores)")]
    public string[] secuenciaCorrecta = { "Violeta", "Amarillo", "Lila", "Verde", "Blanco" };

    // Lista donde almacenamos los colores que seleccionó el jugador
    private List<string> secuenciaJugador = new List<string>();

    // Llamar desde cada locker al presionar E:
    public void ActivarColor(string color)
    {
        if (secuenciaCorrecta == null || secuenciaCorrecta.Length == 0)
        {
            Debug.LogWarning("LockersCOLORS: la secuencia correcta está vacía.");
            return;
        }

        // Normalizamos el string (quitamos espacios y comparamos case-insensitive)
        string input = color.Trim();
        int idx = secuenciaJugador.Count; // índice que vamos a agregar/comprobar

        // Agregamos el color a la secuencia del jugador
        secuenciaJugador.Add(input);
        Debug.Log($"Jugador eligió: '{input}' (pos {idx})");

        // Comparamos inmediatamente el valor recién agregado con el esperado en la secuencia
        if (!string.Equals(secuenciaJugador[idx], secuenciaCorrecta[idx], System.StringComparison.OrdinalIgnoreCase))
        {
            // Error: reiniciamos la secuencia del jugador
            Debug.Log($"Color incorrecto en la posición {idx}. Esperado '{secuenciaCorrecta[idx]}', recibido '{secuenciaJugador[idx]}'. Reiniciando.");
            ReiniciarSecuencia();
            CartelIncorrecto.SetActive(true);
            Invoke(nameof(DesactivarCartelIncorrecto), 2f);
            sonarIncorrecto();
            // Si querés retroalimentación visual/sonora, llamala aquí.
            return;
        }

        // Si llegó hasta acá, el input en idx fue correcto.
        Debug.Log($"Color correcto en posición {idx}: {input}");
        sonarCorrecto();

        // Si el jugador completó la misma longitud que la secuencia correcta -> éxito
        if (secuenciaJugador.Count >= secuenciaCorrecta.Length)
        {
            bool igual = true;
            // (opcional) comprobación completa redundante por seguridad
            for (int i = 0; i < secuenciaCorrecta.Length; i++)
            {
                if (!string.Equals(secuenciaJugador[i].Trim(), secuenciaCorrecta[i].Trim(), System.StringComparison.OrdinalIgnoreCase))
                {
                    igual = false;
                    break;
                }
            }

            if (igual)
            {
                Debug.Log("¡Secuencia completa y correcta! Abriendo puertas...");
                AbrirPuertas();
            }
            else
            {
                Debug.Log("Algo raro: la longitud coincide pero la comparación completa falló. Reiniciando secuencia por seguridad.");
            }

            ReiniciarSecuencia();
        }
    }
    private void DesactivarCartelIncorrecto()
    {
        CartelIncorrecto.SetActive(false);
    }
    private void AbrirPuertas()
    {
        if (puerta1 != null)
        {
            FindAnyObjectByType<MetricasJuego>().CompletarMision("Puzzle Lockers Colores Completado");
            puerta1.SetActive(false);
            CartelCorrecto.SetActive(true);
            Invoke(nameof(DesactivarCartelCorrecto), 2f);
            Debug.Log("Puerta 1 abierta.");
            Trofeo.SetActive(true);
            sonarCorrecto();
        }
    }


    private void DesactivarCartelCorrecto()
    {
        CartelCorrecto.SetActive(false);
    }
    // Reinicia la secuencia del jugador (visualmente puedes resetear luces, etc.)
    private void ReiniciarSecuencia()
    {
        FindAnyObjectByType<MetricasJuego>().FallarMision("Puzzle Lockers Colores Fallido");
        secuenciaJugador.Clear();
        // Aquí podés añadir: ResetVisuales(), PlayErrorSound(), animaciones, etc.
        Debug.Log("Secuencia del jugador reiniciada.");
    }

    // Métodos públicos útiles (opcional)
    public void ForzarReinicio()
    {
        ReiniciarSecuencia();
    }
    public void sonarCorrecto()
    {
        Debug.Log("🔊 SonarCorrecto ejecutado");
        audioSource.PlayOneShot(sonidoCorrecto);
    }

    public void sonarIncorrecto()
    {
        Debug.Log("🔊 SonarIncorrecto ejecutado");
        audioSource.PlayOneShot(sonidoIncorrecto);
    }
}
