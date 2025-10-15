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

    [Header("Patrulla Random")]
    public float RangoPatrulla = 10f;
    public float VelocidadPatrulla = 2f;
    public float TiempoEspera = 2f;
    [Range(0f, 1f)] public float ProbabilidadDeParar = 0.3f;
    private bool esperando = false;

    [Header("Gato / Jugador")]
    public float RangoVision = 10f;
    public Transform Objetivo;
    public Text textoAlerta;
    public ComboMinijuego comboMinijuego;

    [Header("Detección y Cooldown")]
    public float tiempoGracia = 10f;
    private bool enCooldown = false;

    [Header("Drop de ítem")]
    public GameObject itemADroppear;

    // Nueva bandera para validar si el jugador está en rango
    private bool jugadorEnRango = false;
    private bool PARAMISION = false;
    private Misiones missionManager;

    void Start()
    {
        if (Agente == null)
            Agente = GetComponent<NavMeshAgent>();

        IrAPuntoAleatorio();

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

        // Patrulla
        if (!esperando && !Agente.pathPending && Agente.remainingDistance <= Agente.stoppingDistance)
        {
            if (Random.value < ProbabilidadDeParar)
                StartCoroutine(Esperar());
            else
                IrAPuntoAleatorio();
        }

        if (enCooldown) return;

        float distanciaGato = Vector3.Distance(transform.position, Objetivo.position);
        PlayerController gato = Objetivo.GetComponent<PlayerController>();

        // Actualizamos si el jugador está en rango
        jugadorEnRango = (gato != null && distanciaGato <= RangoVision);

        if (jugadorEnRango)
        {
            gato.ReducirAtencionLento();
            MostrarTextoAyuda("¡Presiona E para distraer al guardia!");
            // Si el jugador presiona E y no está ya en una distracción SIII TE ENCONTREE!!!
            if (Input.GetKeyDown(KeyCode.E) && !gato.DistraccionActiva)
            {
                if (missionManager.misionActual == 1)
                {
                    PARAMISION = true;
                }
                gato.DistraccionActiva = true;

                // Activar minijuego
                comboMinijuego.gameObject.SetActive(true);
                comboMinijuego.Activar(gato);

                LimpiarTextoAyuda();
            }
        }
        else
        {
            LimpiarTextoAyuda();
        }
    }

    //  Este método se llama al terminar el combo (bien o mal)
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
        LimpiarTextoAyuda();
    }

    //  Coroutine de cooldown / periodo de gracia
    IEnumerator CooldownDeteccion()
    {
        enCooldown = true;
        yield return new WaitForSeconds(tiempoGracia);
        enCooldown = false;
    }

    public void MostrarTextoAyuda(string mensaje)
    {
        if (textoAlerta != null)
        {
            textoAlerta.gameObject.SetActive(true);
            textoAlerta.text = mensaje;
        }
    }

    public void LimpiarTextoAyuda()
    {
        if (textoAlerta != null)
        {
            textoAlerta.text = "";
            textoAlerta.gameObject.SetActive(false);
        }
    }

    IEnumerator Esperar()
    {
        esperando = true;
        yield return new WaitForSeconds(TiempoEspera);
        IrAPuntoAleatorio();
        esperando = false;
    }

    void IrAPuntoAleatorio()
    {
        Vector3 randomDir = Random.insideUnitSphere * RangoPatrulla + transform.position;

        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, RangoPatrulla, NavMesh.AllAreas))
        {
            Agente.speed = VelocidadPatrulla;
            Agente.SetDestination(hit.position);
            anim.Play(Correr);
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
