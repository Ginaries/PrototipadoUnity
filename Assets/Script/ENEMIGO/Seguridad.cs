using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Seguridad : MonoBehaviour
{
    [Header("Componentes")]
    public NavMeshAgent Agente;

    [Header("Patrulla Random")]
    public float RangoPatrulla = 10f;   // radio en el que se mueve al azar
    public float VelocidadPatrulla = 2f;
    public float TiempoEspera = 2f;     // cuánto se queda quieto cuando decide parar
    [Range(0f, 1f)] public float ProbabilidadDeParar = 0.3f; // 30% de chance de detenerse
    private bool esperando = false;

    [Header("Gato")]
    public float RangoVision = 10f;
    public Transform Objetivo; // referencia al gato

    void Start()
    {
        if (Agente == null)
            Agente = GetComponent<NavMeshAgent>();

        IrAPuntoAleatorio(); // arranca moviéndose
    }

    void Update()
    {
        // detectar al gato
        float distanciaGato = Vector3.Distance(transform.position, Objetivo.position);
        PlayerController gato = Objetivo.GetComponent<PlayerController>();

        if (gato != null && distanciaGato <= RangoVision)
        {
            gato.ReducirAtencionGradual();
        }

        // cuando llega a destino
        if (!esperando && !Agente.pathPending && Agente.remainingDistance <= Agente.stoppingDistance)
        {
            // decide si se queda quieto o sigue de inmediato
            if (Random.value < ProbabilidadDeParar)
            {
                StartCoroutine(Esperar());
            }
            else
            {
                IrAPuntoAleatorio(); // sigue caminando sin parar
            }
        }
    }

    IEnumerator Esperar()
    {
        esperando = true;
        Debug.Log("Guardia se detuvo un momento.");
        yield return new WaitForSeconds(TiempoEspera);
        IrAPuntoAleatorio();
        esperando = false;
    }

    void IrAPuntoAleatorio()
    {
        Vector3 randomDir = Random.insideUnitSphere * RangoPatrulla;
        randomDir += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDir, out hit, RangoPatrulla, NavMesh.AllAreas))
        {
            Agente.speed = VelocidadPatrulla;
            Agente.SetDestination(hit.position);
            Debug.Log("Guardia yendo a: " + hit.position);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, RangoVision);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, RangoPatrulla);
    }
}

