using UnityEngine;

public class ZonaSegura : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.SetInSafeZone(true);
            Debug.Log("Jugador entró a la zona segura.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.SetInSafeZone(false);
            Debug.Log("Jugador salió de la zona segura.");
        }
    }
}
