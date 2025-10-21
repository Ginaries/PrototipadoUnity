using UnityEngine;
using UnityEngine.UI;

public class GOALcs : MonoBehaviour
{
    public GameObject Puerta;
    public GameObject Puerta2;
    public GameObject Trofeo; // el cartel
    private bool ConsiguioElTrofeo = false;
    public GameObject CartelPuertaAbierta; // el cartel

    private async void OnTriggerEnter(Collider other)
    {
        if (ConsiguioElTrofeo == false){
            if (other.CompareTag("PELOTA SALA 1"))
            {
                Puerta.SetActive(false);
                Puerta2.SetActive(false);
                Trofeo.SetActive(true); // mostrar el trofeo
                //DISPARAR SONIDO DE TROFEO CONSEGUIDO
                CartelPuertaAbierta.SetActive(true); // ✅ mostrar el cartel

                await System.Threading.Tasks.Task.Delay(2000);

                CartelPuertaAbierta.SetActive(false); // ✅ ocultarlo después de 2 segundos
            }
        }
    }
}
