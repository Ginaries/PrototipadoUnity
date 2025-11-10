using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactuable : MonoBehaviour
{
    private Rigidbody rb;
    public float pushForce = 5f;
    private bool jugadorCerca = false;

    public PlayerController player;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            Vector3 direccion = (transform.position - player.transform.position).normalized;
            Interactuar(direccion);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            player.MostrarCartel("EMPUJAR");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            player.OcultarCartel("EMPUJAR");
        }
    }

    public void Interactuar(Vector3 direccion)
    {
        rb.AddForce(direccion * pushForce, ForceMode.Impulse);
    }
}