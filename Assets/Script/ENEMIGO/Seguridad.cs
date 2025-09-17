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

        // Detectar al jugador
        if (Distancia < RangoVision)
        {
            Persiguiendo = true;
            if (Distancia < 2f)
            {
                ComboMinijuego combo = FindObjectOfType<ComboMinijuego>();
                if (combo != null)
                {
                    combo.Activar(Objetivo.GetComponent<PlayerController>());
                }
            }
        }
        else if (Distancia > RangoVision + 3f)
        {
            Persiguiendo = false;
        }

        // Comportamiento del enemigo
        if (Persiguiendo == false)
        {
            Agente.speed = 0;
            anim.CrossFade(idle);
        }
        else if (Persiguiendo == true)
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
