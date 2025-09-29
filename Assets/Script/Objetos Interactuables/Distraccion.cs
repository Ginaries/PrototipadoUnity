using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Distraccion : MonoBehaviour
{
    [Header("Distracción")]
    public int puntosARestar = 2;          // cuánto baja la atención al acercarse
    public float radio = 2f;               // rango de detección
    public bool dibujarGizmo = true;       // para ver el radio en escena

    [Header("Reacción Física")]
    public float fuerzaEmpuje = 6f;        // fuerza con la que se aleja del jugador
    public float torque = 3f;              // cuánta rotación aplica al rodar
    public float cooldownEmpuje = 1.5f;    // evita aplicar fuerza constantemente

    private PlayerController gato;
    private Rigidbody rb;
    private bool estabaDentro = false;
    private float proximoEmpuje = 0f;

    void Start()
    {
        gato = FindFirstObjectByType<PlayerController>();
        rb = GetComponent<Rigidbody>();

        if (gato == null)
            Debug.LogWarning("Distraccion: no encontré PlayerController en escena");

        if (rb == null)
            Debug.LogError("Distraccion necesita un Rigidbody en el mismo objeto!");
        else
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }

    void Update()
    {
        if (gato == null || rb == null) return;
        if (gato.DistraccionActiva) return;

        float distancia = Vector3.Distance(transform.position, gato.transform.position);
        bool dentro = distancia <= radio;

        if (dentro && !estabaDentro)
        {
            // 🔹 Bajar atención
            gato.AtencionActual = Mathf.Max(gato.AtencionActual - puntosARestar, 0);
            gato.ActualizarBarraAtencion();
            Debug.Log("Distracción activada -> atención actual: " + gato.AtencionActual);

            // 🔹 Activar combo si llega a 0
            if (gato.AtencionActual <= 0)
            {
                gato.DistraccionActiva = true;
                if (gato.comboMinijuego != null)
                {
                    gato.comboMinijuego.Activar(gato);
                }
            }

            // 🔹 Empujar físicamente la esfera
            if (Time.time >= proximoEmpuje)
            {
                Vector3 direccionEmpuje = (transform.position - gato.transform.position).normalized + Vector3.up * 0.3f;
                rb.AddForce(direccionEmpuje * fuerzaEmpuje, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * torque, ForceMode.Impulse);
                proximoEmpuje = Time.time + cooldownEmpuje;
            }

            estabaDentro = true;
        }

        if (!dentro && estabaDentro)
        {
            estabaDentro = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!dibujarGizmo) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radio);
    }
}
