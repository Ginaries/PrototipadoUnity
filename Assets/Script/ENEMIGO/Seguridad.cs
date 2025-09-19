using UnityEngine;

using UnityEngine.AI;

public class Seguridad : MonoBehaviour
{
    public NavMeshAgent Agente;
    public float VelocidadPatrulla = 3.5f;
    public bool Persiguiendo = false;
    public float RangoVision = 10f;
    public float Distancia;
    public Transform Objetivo;

    [Header("Animaciones")]
    public Animation anim;
    public string Correr;
    public string idle;


    void Update()
    {
        Distancia = Vector3.Distance(Agente.transform.position, Objetivo.position);

        PlayerController player = Objetivo.GetComponent<PlayerController>();

        if (player != null && player.IsInTheBox())
        {
            // Si el jugador está en la caja segura, no lo persigue
            Persiguiendo = false;
            Agente.speed = 0;
            anim.CrossFade(idle);
            return; // salir del Update
        }

        // Detectar al jugador solo si no está en la zona segura
        if (Distancia < RangoVision)
        {
            Persiguiendo = true;
            if (Distancia < 2f)
            {
                ComboMinijuego combo = FindObjectOfType<ComboMinijuego>();
                if (combo != null && !combo.EstaActivo())
                {
                    combo.Activar(player);
                }
            }
        }
        else if (Distancia > RangoVision + 3f)
        {
            Persiguiendo = false;
        }

        // Comportamiento del enemigo
        if (!Persiguiendo)
        {
            Agente.speed = 0;
            anim.CrossFade(idle);
        }
        else
        {
            Agente.speed = VelocidadPatrulla;
            Agente.SetDestination(Objetivo.position);
            anim.CrossFade(Correr);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Agente.transform.position, RangoVision);

    }
}
