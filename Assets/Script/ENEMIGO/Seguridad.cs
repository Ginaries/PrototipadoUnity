using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.UI;

public class Seguridad : MonoBehaviour
{
    [Header("Componentes")]
    public NavMeshAgent Agente;

    [Header("Animaciones")]
    public Animation anim;
    public string Correr;
    public string idle;

    [Header("Patrulla por puntos específicos")]
    public Transform[] puntosPatrulla; // 🔹 Lista de puntos vacíos (asigná desde el inspector)
    public float VelocidadPatrulla = 2f;
    public float TiempoEspera = 2f;
    private bool esperando = false;

    [Header("Gato / Jugador")]
    public float RangoVision = 10f;
    public Transform Objetivo;
    public ComboMinijuego comboMinijuego;

    [Header("Detección y Cooldown")]
    public float tiempoGracia = 10f;
    private bool enCooldown = false;

    [Header("Drop de ítem")]
    public GameObject itemADroppear;

    private bool jugadorEnRango = false;
    private bool PARAMISION = false;
    private Misiones missionManager;

    void Start()
    {
        if (Agente == null)
            Agente = GetComponent<NavMeshAgent>();

        // 🔹 Ir a un punto de patrulla inicial
        IrAPuntoPatrulla();

        if (comboMinijuego != null)
        {
            comboMinijuego.OnComboTerminado += OnComboTerminado;
        }
    }

    [System.Obsolete]
    void Update()
    {
        if (missionManager == null)
        {
            missionManager = FindObjectOfType<Misiones>();
        }

        // Animaciones
        if (Agente.velocity.magnitude > 0.1f)
            anim.CrossFade(Correr, 0.2f);
        else
            anim.CrossFade(idle, 0.2f);

        // 🔹 Patrulla por puntos fijos
        if (!esperando && !Agente.pathPending && Agente.remainingDistance <= Agente.stoppingDistance)
        {
            StartCoroutine(Esperar());
        }

        if (enCooldown) return;

        float distanciaGato = Vector3.Distance(transform.position, Objetivo.position);
        PlayerController gato = Objetivo.GetComponent<PlayerController>();

        jugadorEnRango = (gato != null && distanciaGato <= RangoVision);

        if (jugadorEnRango)
        {
            gato.ReducirAtencionLento();

            if (Input.GetKeyDown(KeyCode.E) && !gato.DistraccionActiva)
            {
                if (missionManager.misionActual == 1)
                    PARAMISION = true;

                gato.DistraccionActiva = true;
                comboMinijuego.gameObject.SetActive(true);
                comboMinijuego.Activar(gato);

            }
        }
    }

    // 🔹 Ir a un punto vacío del array
    void IrAPuntoPatrulla()
    {
        if (puntosPatrulla.Length == 0)
            return;

        int indice = Random.Range(0, puntosPatrulla.Length);
        Transform destino = puntosPatrulla[indice];

        Agente.speed = VelocidadPatrulla;
        Agente.SetDestination(destino.position);
        anim.Play(Correr);
    }

    IEnumerator Esperar()
    {
        esperando = true;
        yield return new WaitForSeconds(TiempoEspera);
        IrAPuntoPatrulla(); // 🔹 Luego va al siguiente punto
        esperando = false;
    }

    public void OnComboTerminado(bool exito)
    {
        if (exito && itemADroppear != null && jugadorEnRango)
        {
            Instantiate(itemADroppear, transform.position + Vector3.right, Quaternion.identity);
        }
        if (PARAMISION)
        {
            missionManager.CompletarMision();
            PARAMISION = false;
        }
        StartCoroutine(CooldownDeteccion());

    }

    IEnumerator CooldownDeteccion()
    {
        enCooldown = true;
        yield return new WaitForSeconds(tiempoGracia);
        enCooldown = false;
    }

 

    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, RangoVision);
    }
}
