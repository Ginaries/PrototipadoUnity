using UnityEngine;
using UnityEngine.UI;
public class Trepable : MonoBehaviour
{
    [Tooltip("Punto exacto donde el player quedará después de trepar. Opcional")]
    public Transform climbPoint;
    public Text TextoAyuda; // Que puedo hacer con este objeto
    public float radioDeteccion = 1.5f; // Ajusta el radio según tu necesidad

    private bool jugadorCerca = false;

    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radioDeteccion);
        bool playerDetectado = false;

        foreach (var col in colliders)
        {
            if (col.CompareTag("Player"))
            {
                playerDetectado = true;
                break;
            }
        }

        if (playerDetectado && !jugadorCerca)
        {
            jugadorCerca = true;
            cerca();
        }
        else if (!playerDetectado && jugadorCerca)
        {
            jugadorCerca = false;
            if (TextoAyuda != null)
                TextoAyuda.gameObject.SetActive(false);
        }
    }
    void cerca()
    {
        if (TextoAyuda != null)
        {
            TextoAyuda.gameObject.SetActive(true);
            TextoAyuda.text = "Presiona 'Shift' para trepar";
        }
    }
    public Vector3 GetPosicionTrepada()
    {
        if (climbPoint != null) return climbPoint.position;

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Vector3 top = col.bounds.center + Vector3.up * col.bounds.extents.y;
            return top + transform.forward * 0.5f; // un poco adelante del borde
        }

        return transform.position + Vector3.up * 1.5f;
    }
}
