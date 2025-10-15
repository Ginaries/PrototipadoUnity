using UnityEngine;

public class Trepable : MonoBehaviour
{
    [Tooltip("Punto exacto donde el player quedará después de trepar. Opcional")]
    public Transform climbPoint;
    public float radioDeteccion = 1.5f; 

    private bool jugadorCerca = false;
    private GameObject jugadorDetectado;

    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radioDeteccion);
        bool playerDetectado = false;
        GameObject playerObj = null;

        foreach (var col in colliders)
        {
            if (col.CompareTag("Player"))
            {
                playerDetectado = true;
                playerObj = col.gameObject;
                break;
            }
        }

        if (playerDetectado && !jugadorCerca)
        {
            jugadorCerca = true;
            jugadorDetectado = playerObj;
            MostrarBurbuja(playerObj);
        }
        else if (!playerDetectado && jugadorCerca)
        {
            jugadorCerca = false;
            OcultarBurbuja(jugadorDetectado);
            jugadorDetectado = null;
        }
    }

    void MostrarBurbuja(GameObject player)
    {
        var playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
            playerController.MostrarCartel("Trepar");
    }

    void OcultarBurbuja(GameObject player)
    {
        if (player == null) return;

        var playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
            playerController.OcultarCartel("Trepar");
    }

    public Vector3 GetPosicionTrepada()
    {
        if (climbPoint != null) return climbPoint.position;

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Vector3 top = col.bounds.center + Vector3.up * col.bounds.extents.y;
            return top + transform.forward * 0.5f;
        }

        return transform.position + Vector3.up * 1.5f;
    }
}