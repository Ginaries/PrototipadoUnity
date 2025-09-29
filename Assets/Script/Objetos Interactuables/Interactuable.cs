using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Interactuable : MonoBehaviour
{
    private Rigidbody rb;
    public Text TextoAyuda;
    public float pushForce = 5f;
    private bool jugadorCerca = false;
    private float radioDeteccion = 1.5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

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
            MostrarAyuda();
        }
        else if (!playerDetectado && jugadorCerca)
        {
            jugadorCerca = false;
            if (TextoAyuda != null)
                TextoAyuda.gameObject.SetActive(false);
        }
    }

    void MostrarAyuda()
    {
        if (TextoAyuda != null)
        {
            TextoAyuda.gameObject.SetActive(true);
            TextoAyuda.text = "Presiona 'E' para empujar";
        }
    }

    public void Interactuar(Vector3 direccion)
    {
        rb.AddForce(direccion * pushForce, ForceMode.Impulse);
    }
}
