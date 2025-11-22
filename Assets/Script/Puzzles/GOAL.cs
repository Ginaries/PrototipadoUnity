using UnityEngine;
using UnityEngine.UI;

public class GOALcs : MonoBehaviour
{
    public GameObject Puerta;
    public GameObject Puerta2;
    public GameObject Trofeo; // el cartel
    private bool ConsiguioElTrofeo = false;
    public GameObject CartelPuertaAbierta; // el cartel
    public AudioSource audioSource;
    public AudioClip correctoSound;


    [System.Obsolete]
    private async void OnTriggerEnter(Collider other)
    {
        if (ConsiguioElTrofeo == false){
            if (other.CompareTag("PELOTA SALA 1"))
            {
                Puerta.SetActive(false);
                Puerta2.SetActive(false);
                Trofeo.SetActive(true); // mostrar el trofeo
                //DISPARAR SONIDO DE TROFEO CONSEGUIDO
                CartelPuertaAbierta.SetActive(true); // mostrar el cartel
                audioSource.PlayOneShot(correctoSound);
                await System.Threading.Tasks.Task.Delay(2000);
                ConsiguioElTrofeo = true;
                FindObjectOfType<MetricasJuego>().CompletarMision("Mision 1 - GOL ");
                CartelPuertaAbierta.SetActive(false); // ocultarlo después de 2 segundos
            }
        }
    }
}
