using UnityEngine;

public class LockerColor : MonoBehaviour
{
    public LockersCOLORS GestorLockers;
    public GameObject CartelInteraccion;

    public string Color;

    private bool jugadorDentro = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = true;
            CartelInteraccion.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = false;
            CartelInteraccion.SetActive(false);
        }
    }

    private void Update()
    {
        if (jugadorDentro && Input.GetKeyDown(KeyCode.E))
        {
            GestorLockers.ActivarColor(Color);
        }
    }
}
