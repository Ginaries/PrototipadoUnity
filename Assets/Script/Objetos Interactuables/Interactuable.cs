using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Interactuable : MonoBehaviour
{
    private Rigidbody rb;

    public float pushForce = 5f; // fuerza del empuje

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Interactuar(Vector3 direccion)
    {
        Debug.Log("Empujando: " + gameObject.name);

        // Aplicar fuerza en la dirección indicada
        rb.AddForce(direccion * pushForce, ForceMode.Impulse);
    }
}
