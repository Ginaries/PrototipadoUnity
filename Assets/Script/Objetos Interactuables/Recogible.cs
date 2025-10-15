using UnityEngine;

public class Recogible : MonoBehaviour
{
    public Inventario inventario;
    private Rigidbody rb;
    public string nombreObjeto = "Caja";

    private bool jugadorCerca = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Detecta si el jugador está dentro del radio de 1.5 unidades
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1.5f);
        bool playerDetectado = false;

        foreach (var col in colliders)
        {
            if (col.CompareTag("Player"))
            {
                playerDetectado = true;
                break;
            }
        }

        // Actualiza el estado de cercanía
        jugadorCerca = playerDetectado;

        // Si el jugador está cerca y presiona E, recoge el objeto
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            if (inventario != null)
            {
                inventario.AgregarObjeto(nombreObjeto);
            }

            Destroy(gameObject); // Elimina el objeto de la escena
        }
    }

    // --- PUERTA ---
    [Header("Referencia a la puerta")]
    public PUERTA puerta;

    public void EsLlave()
    {
        if (puerta != null)
        {
            puerta.TieneLlave();
            Misiones.Instance.CompletarMision();
        }
        else
        {
            Debug.LogWarning("⚠ No se encontró una puerta en la escena");
        }
    }

    void OnDestroy()
    {
        if (nombreObjeto == "Llave")
        {
            EsLlave();
        }
    }
}