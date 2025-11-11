using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class moverpinguino : MonoBehaviour
{
    public float velocidad = 5f;
    public float fuerzaSalto = 5f;
    private Rigidbody rb;
    private bool enSuelo = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    
    void Update()
    {
        // Movimiento
        float movX = Input.GetAxis("Horizontal2");
        float movZ = Input.GetAxis("Vertical2");

        Vector3 movimiento = new Vector3(movX, 0, movZ) * velocidad;
        Vector3 nuevaVelocidad = new Vector3(movimiento.x, rb.linearVelocity.y, movimiento.z);
        rb.linearVelocity = nuevaVelocidad;

        // Salto
        if (Input.GetButtonDown("Jump2") && enSuelo)
        {
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
            enSuelo = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Detectar si toca el suelo
        if (collision.contacts.Length > 0)
        {
            if (collision.contacts[0].normal.y > 0.5f)
                enSuelo = true;
        }
    }
}
