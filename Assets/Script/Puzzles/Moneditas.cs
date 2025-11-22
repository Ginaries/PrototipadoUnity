using UnityEngine;

public class Moneditas : MonoBehaviour
{

    public GameObject Trofeo;
    public GameObject Collage;
    public AudioClip audioClip;
    public AudioSource audioSource;
    public GameObject cartel1;
    public GameObject cartel2;


    public int contadorMonedas = 0;
    public int DesbloquearTrofeo = 10;
    public bool finalizado = false;

    void Update()
    {
        if (contadorMonedas >= DesbloquearTrofeo && finalizado == false)
        {
            FindAnyObjectByType<MetricasJuego>().CompletarMision("Mision 4 - Recolectar Monedas Completado");
            Trofeo.SetActive(true);
            Collage.SetActive(true);
            finalizado = true;
            audioSource.PlayOneShot(audioClip);
            Invoke("MostrarPrimerCartel", 2f);
        }

    }
    void MostrarPrimerCartel()
    {
        cartel1.SetActive(true);
        Invoke("MostrarSegundoCartel", 4f);
    }
    public void MostrarSegundoCartel()
    {
        cartel1.SetActive(false);
        cartel2.SetActive(true);
        Invoke("OcultarSegundoCartel", 4f);
    }
    public void OcultarSegundoCartel()
    {
        cartel2.SetActive(false);
    }
}
