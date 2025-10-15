using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactuable : MonoBehaviour
{
    private Rigidbody rb;
    public float pushForce = 5f;
    private bool jugadorCerca = false;
    private float radioDeteccion = 1.5f;
    public PlayerController player;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (player == null) return;

        float distancia = Vector3.Distance(transform.position, player.transform.position);

        if (distancia <= radioDeteccion && !jugadorCerca)
        {
            jugadorCerca = true;
            player.MostrarCartel("EMPUJAR");
        }
        else if (distancia > radioDeteccion && jugadorCerca)
        {
            jugadorCerca = false;
            player.OcultarCartel("EMPUJAR");
        }

        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            Vector3 direccion = (transform.position - player.transform.position).normalized;
            Interactuar(direccion);
        }
    }

    public void Interactuar(Vector3 direccion)
    {
        rb.AddForce(direccion * pushForce, ForceMode.Impulse);
    }
}