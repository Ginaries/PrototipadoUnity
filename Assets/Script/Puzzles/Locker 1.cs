using UnityEngine;

public class LockerColor : MonoBehaviour
{
    public LockersCOLORS GestorLockers;
    public string Color;

    private bool jugadorDentro = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = false;
        }
    }

    private void Update()
    {
        if (jugadorDentro && Input.GetKeyDown(KeyCode.E))
        {
            GestorLockers.ActivarColor(Color);
            Debug.Log("Locker activado: " + Color);
        }
    }
}
