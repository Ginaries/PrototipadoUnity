using UnityEngine;

public class PlacaDetector : MonoBehaviour
{
    public int idPlaca; // 1 o 2
    public GestorPuzzle4 gestor;
   

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (idPlaca == 1)
                gestor.Placa1Pisada(true);
            if (idPlaca == 2)
                gestor.Placa2Pisada(true);
     
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (idPlaca == 1)
                gestor.Placa1Pisada(false);

            if (idPlaca == 2)
                gestor.Placa2Pisada(false);

        }
    }
}
