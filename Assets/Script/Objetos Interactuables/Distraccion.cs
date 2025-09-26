using UnityEngine;

public class Distraccion : MonoBehaviour
{
    [Header("Distraccion")]
    public int puntosARestar = 2;      // cuanto baja la atencion al acercarse
    public float radio = 2f;           // rango mas chico que el guardia
    public bool dibujarGizmo = true;   // para ver el radio en escena

    private PlayerController gato;
    private bool estabaDentro = false; // aplicar una sola vez por acercamiento

    void Start()
    {
        gato = FindFirstObjectByType<PlayerController>();
        if (gato == null)
            Debug.LogWarning("Distraccion: no encontre PlayerController en escena"); //ante la duda ah
    }

    void Update()
    {
        if (gato == null) return;
        if (gato.DistraccionActiva) return;

        float distancia = Vector3.Distance(transform.position, gato.transform.position);
        bool dentro = distancia <= radio;

        if (dentro && !estabaDentro)
        {
            gato.AtencionActual = Mathf.Max(gato.AtencionActual - puntosARestar, 0);
            Debug.Log("Distraccion activada -> atencion actual: " + gato.AtencionActual);

            if (gato.AtencionActual <= 0)
            {
                gato.DistraccionActiva = true;
                if (gato.comboMinijuego != null)
                {
                    gato.comboMinijuego.Activar(gato);
                }
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

