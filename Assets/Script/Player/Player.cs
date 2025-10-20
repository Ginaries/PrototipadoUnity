using UnityEngine;

public class Player : MonoBehaviour
{
    public float velocidad = 5f;
    public float sensibilidadRotacion = 200f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.freezeRotation = true; // evita que se caiga o gire al chocar
    }

    void Update()
    {
        Mover();
    }

    void Mover()
    {
        float horizontal = Input.GetAxis("Horizontal");  // A/D o ←/→
        float vertical = Input.GetAxis("Vertical");      // W/S o ↑/↓

        Vector3 direccion = new Vector3(horizontal, 0, vertical).normalized;

        if (direccion.magnitude >= 0.1f)
        {
            // Gira suavemente hacia la dirección de movimiento según la cámara
            float anguloObjetivo = Mathf.Atan2(direccion.x, direccion.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            Quaternion rotacion = Quaternion.Euler(0f, anguloObjetivo, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotacion, Time.deltaTime * sensibilidadRotacion / 100f);

            // Movimiento hacia adelante en la dirección del jugador
            Vector3 moverDir = Quaternion.Euler(0f, anguloObjetivo, 0f) * Vector3.forward;
            transform.position += moverDir.normalized * velocidad * Time.deltaTime;
        }
    }
}
