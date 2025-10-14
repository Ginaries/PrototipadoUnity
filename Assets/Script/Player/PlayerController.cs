using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    //no me aparece en el inspector
    [Header("Misiones")]
    public Misiones missionManager;
    public GameObject llavePerdida; // referencia al objeto llave
    public GameObject guardia; // referencia al guardia

    [Header("Movimiento")]
    public float speed = 5f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;
    private Vector3 velocity;
    private Vector3 currentVelocity = Vector3.zero;


    [Header("Cámara")]
    public Transform cameraTransform;
    public float mouseSensitivity = 3f;
    public float distanceFromPlayer = 2.5f;
    public float cameraHeight = 2f;

    [Header("Trepar")]
    public float climbDistance = 1.8f;
    public float climbSpeed = 2f;
    private bool isClimbing = false;
    private Collider climbingCol;

    [Header("Combo")]
    public ComboMinijuego comboMinijuego;

    [Header("Atención")]
    public float AtencionMax = 10f;
    public float AtencionActual;
    public bool DistraccionActiva = false;

    private float tiempoReduccion = 1f; // cada 1 seg baja 1 punto (la tiro, dsps se cambia si no xd)
    private float proximoTick = 0f;

    //Barra de atención en GUI
    public Image barradeatencion;

    private CharacterController controller;
    private float yaw;
    private float pitch;
    private bool isInTheBox = false; // jugador en zona segura

// 👇 --- Sistema de burbuja de interacción ---
[Header("Burbuja de Interacción")]
public GameObject bubbleUI;         // Asigná aquí el objeto de UI con el PNG
public Transform bubbleAnchor;      // Un Empty encima de la cabeza
public Camera mainCamera;

private RectTransform bubbleRect;
    private bool bubbleVisible = false;

    public bool IsInTheBox()
    {
        return isInTheBox;
    }

    public void ActualizarBarraAtencion()
    {
        if (barradeatencion != null)
            barradeatencion.fillAmount = AtencionActual / AtencionMax;
    }

    public void SetInSafeZone(bool state)
    {
        isInTheBox = state;
    }
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        AtencionActual = AtencionMax;

        
    if (bubbleUI != null)
    {
        bubbleRect = bubbleUI.GetComponent<RectTransform>();
        bubbleUI.SetActive(false);
    }

    if (mainCamera == null)
        mainCamera = Camera.main;
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.M))
            Debug.Log("M está siendo presionada");

        ReducirAtencionLento();
        ActualizarBarraAtencion();

        // 💤 Si está distraído, el jugador no controla al gato, pero éste se mueve solo
        if (DistraccionActiva)
        {
            ComportamientoDistraido();
            return;
        }

        HandleMovement();
        HandleCamera();
        Saltar();
        AgarrarCosas();
        Trepar();

        
    if (bubbleVisible && bubbleUI != null)
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(bubbleAnchor.position);
        bubbleRect.position = screenPos;
    }

    }
    private float tiempoCambioDireccion = 0f;
    private Vector3 direccionAleatoria = Vector3.zero;
    public void ShowBubble()
    {
        if (bubbleUI != null)
        {
            Debug.Log("ShowBubble llamado");
            if (bubbleUI != null)
            {
                bubbleUI.SetActive(true);
                bubbleVisible = true;
            }
        }
    }

public void HideBubble()
{
    if (bubbleUI != null)
    {
        bubbleUI.SetActive(false);
        bubbleVisible = false;
    }
}
    void ComportamientoDistraido()
    {
        // Cambiar de dirección cada cierto tiempo
        if (Time.time >= tiempoCambioDireccion)
        {
            tiempoCambioDireccion = Time.time + Random.Range(1f, 3f);
            direccionAleatoria = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        }

        // mover al gato
        Vector3 movimiento = direccionAleatoria * (speed * 0.5f);
        controller.Move(movimiento * Time.deltaTime);

        // 🧭 girar hacia la dirección de movimiento
        if (movimiento.magnitude > 0.1f)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(movimiento);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, Time.deltaTime * 3f);
        }

        // saltos ocasionales
        if (controller.isGrounded && Random.value < 0.005f)
        {
            velocity.y = Mathf.Sqrt(-2f * gravity * 1.2f);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }


    // --- Variables globales ---
    private float coyoteTime = 0.15f;
    private float coyoteCounter;
    private float jumpForce = 2.0f;
    private float groundedGravity = -2f; // leve presión al suelo
    private float gravityForce = -9.81f;
    private Vector3 moveInput = Vector3.zero;


    // NUEVO: para detectar si estaba en el suelo el frame anterior
    private bool wasGrounded = false;

    // 🧭 --- MOVIMIENTO GENERAL ---
    public void HandleMovement()
    {
        if (isClimbing)
        {
            velocity = Vector3.zero;
            return;
        }

        // --- Entrada del jugador ---
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(h, 0, v).normalized;

        // --- Dirección relativa a la cámara ---
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * v + camRight * h).normalized;

        if (moveDir.sqrMagnitude > 0.01f)
            transform.forward = moveDir;

        Vector3 move = moveDir * speed;

        // --- COYOTE TIME ---
        if (controller.isGrounded)
        {
            coyoteCounter = coyoteTime;
            if (!wasGrounded)
                velocity.y = groundedGravity; // reset vertical
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        // --- GRAVEDAD ---
        if (!controller.isGrounded)
        {
            velocity.y += gravityForce * Time.deltaTime;
        }
        else if (velocity.y < 0f)
        {
            velocity.y = groundedGravity;
        }

        // --- MOVIMIENTO FINAL ---
        Vector3 finalVelocity = move + new Vector3(0, velocity.y, 0);
        controller.Move(finalVelocity * Time.deltaTime);

        wasGrounded = controller.isGrounded;
    }


    // 🐾 --- SALTO SEPARADO ---
    public void Saltar()
    {
        // Evitar saltar si está escalando o distraído
        if (isClimbing || DistraccionActiva)
            return;

        // Saltar solo si se presiona Space y está dentro del tiempo de coyote
        if (Input.GetButtonDown("Jump") && coyoteCounter > 0f)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravityForce);
            coyoteCounter = 0f;
        }
    }


    public void HandleCamera()
    {
        bool mirarObjetivoAhora = Input.GetKey(KeyCode.M);

        Vector3 pivot = transform.position + Vector3.up * 0.5f;
        Quaternion rotation;

        if (mirarObjetivoAhora && missionManager != null)
        {
            Transform objetivo = null;
            int EnQueMisionEstamos = missionManager.misionActual;

            if (EnQueMisionEstamos == 0 && llavePerdida != null)
                objetivo = llavePerdida.transform;
            else if (EnQueMisionEstamos == 1 && guardia != null)
                objetivo = guardia.transform;

            if (objetivo != null)
            {
                // Calcula la dirección hacia el objetivo
                Vector3 direccion = (objetivo.position - pivot).normalized;
                rotation = Quaternion.LookRotation(direccion);

                // Calcula la posición de la cámara detrás del jugador, mirando al objetivo
                Vector3 desiredCameraPos = pivot - direccion * distanceFromPlayer;

                // Opcional: ajusta la altura si quieres
                desiredCameraPos.y = pivot.y + cameraHeight;

                cameraTransform.position = desiredCameraPos;
                cameraTransform.rotation = rotation;
                return; // Salimos para no aplicar la lógica normal
            }
        }

        // --- Lógica normal de cámara ---
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -30f, 60f);

        rotation = Quaternion.Euler(pitch, yaw, 0);

        Vector3 normalOffset = rotation * new Vector3(0, 0, -distanceFromPlayer);
        Vector3 normalCameraPos = pivot + normalOffset;

        // --- Raycast para evitar atravesar paredes ---
        RaycastHit hit;
        float minDistance = 0.5f;
        Vector3 directionNormal = (normalCameraPos - pivot).normalized;
        float maxDistance = distanceFromPlayer;

        if (Physics.SphereCast(pivot, 0.2f, directionNormal, out hit, maxDistance))
        {
            float hitDist = Mathf.Max(hit.distance - 0.1f, minDistance);
            cameraTransform.position = pivot + directionNormal * hitDist;
        }
        else
        {
            cameraTransform.position = normalCameraPos;
        }

        cameraTransform.rotation = rotation;
    }
    public void AgarrarCosas()
    {
        if (isClimbing) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hit, 3f))
            {
                GameObject obj = hit.collider.gameObject;

                // Solo interactuables, NO recogibles
                if (obj.CompareTag("Interactuable"))
                {
                    Interactuable interactuable = obj.GetComponent<Interactuable>();
                    if (interactuable != null)
                    {
                        Vector3 direccionEmpuje = transform.forward;
                        interactuable.Interactuar(direccionEmpuje);
                        Debug.Log("Empujaste: " + obj.name);
                    }
                }
            }
        }
    }
    public void Trepar()
    {
        if (isClimbing)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                // subir hacia arriba mientras mantenga la tecla
                controller.Move(Vector3.up * climbSpeed * Time.deltaTime);

                // comprobar si ya superó la altura del objeto
                if (climbingCol != null && transform.position.y >= climbingCol.bounds.max.y)
                {
                    Vector3 finalPos = new Vector3(transform.position.x,
                                                   climbingCol.bounds.max.y + 0.05f,
                                                   transform.position.z)
                                      + transform.forward * 0.5f;

                    controller.enabled = false;
                    transform.position = finalPos;
                    controller.enabled = true;

                    isClimbing = false;
                    climbingCol = null;
                }
            }
            else
            {
                // si suelta la tecla, cae
                isClimbing = false;
                climbingCol = null;
            }
            return;
        }

        // Detectar inicio de escalada
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            RaycastHit hit;
            Vector3 origen = transform.position + Vector3.up * 0.5f;
            float radius = controller.radius * 0.9f;
            float height = controller.height * 0.5f;

            if (Physics.CapsuleCast(origen, origen + Vector3.up * height, radius, transform.forward, out hit, climbDistance))
            {
                Trepable trep = hit.collider.GetComponent<Trepable>();
                if (trep != null)
                {
                    isClimbing = true;
                    velocity = Vector3.zero;
                    climbingCol = hit.collider;
                }
            }
        }
    }
    public void ReducirAtencionGradual()
    {
        if (!DistraccionActiva && Time.time >= proximoTick)
        {
            proximoTick = Time.time + tiempoReduccion;
            AtencionActual = Mathf.Max(AtencionActual - 1, 0);
            Debug.Log("Atención actual: " + AtencionActual);

            if (AtencionActual <= 0)
            {
                DistraccionActiva = true;
                Debug.Log("El gato está distraído, activar combo!");

                if (comboMinijuego != null)
                {
                    comboMinijuego.Activar(this);
                }
                else
                {
                    Debug.LogError("ERROR");
                }
            }
        }
    }
    // subir atención al completar el minijuego, debe realizar el combo correctamente, el mismo se va a activar cuando el jugador pierda toda la atencion    
    public void AumentarAtencion()
    {
        AtencionActual += 2.0f; // aumentar en 2 puntos
        if (AtencionActual > AtencionMax)
        {
            AtencionActual = AtencionMax;
            DistraccionActiva = false;
            ActualizarBarraAtencion();
            Debug.Log("Atención restaurada al máximo: " + AtencionActual);
        }
        return;

    }
    //reducir la atencion muy lentamente mientras el jugador se mueve de por si, somos un gato, puede perder la atencion para lamerse
    public void ReducirAtencionLento()
    {
        if (!DistraccionActiva && Time.time >= proximoTick)
        {
            proximoTick = Time.time + (tiempoReduccion * 5); // reducir más lento
            AtencionActual = Mathf.Max(AtencionActual - 1.0f, 0); // reducir menos
            Debug.Log("Atención actual (lento): " + AtencionActual);

            if (AtencionActual <= 0)
            {
                DistraccionActiva = true;
                Debug.Log("El gato está distraído, activar combo!");

                if (comboMinijuego != null)
                {
                    comboMinijuego.Activar(this);
                }
                else
                {
                    Debug.LogError("ERROR");
                }
            }
        }
    }
    // ESTE Realizardash ya no se usa, lo dejo por las dudas capaz en un futuro lo necesito
    public void RealizarDash()
    {
        Vector3 dashDir = (transform.position - Object.FindAnyObjectByType<Seguridad>().transform.position).normalized;
        controller.Move(dashDir * 5f); // Ajusta la fuerza del dash
    }
}

