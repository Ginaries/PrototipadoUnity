using System.Collections;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;
    private Vector3 velocity;
    private Vector3 currentVelocity = Vector3.zero;
    public float acceleration = 10f;
    public float deceleration = 8f;

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
    public int AtencionMax = 10;
    public int AtencionActual;
    public bool DistraccionActiva = false;

    private float tiempoReduccion = 1f; // cada 1 seg baja 1 punto (la tiro, dsps se cambia si no xd)
    private float proximoTick = 0f;


    private CharacterController controller;
    private float yaw;
    private float pitch;
     private bool isInTheBox = false; // jugador en zona segura

    public bool IsInTheBox()
    {
        return isInTheBox;
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
    }

    void Update()
    {
        if (DistraccionActiva)
        {
            // Si está distraído, no puede moverse
            return;
        }
        HandleMovement();
        HandleCamera();
        Saltar();
        AgarrarCosas();
        Trepar();
    }
    void OnGUI()
    {
        GUIStyle estilo = new GUIStyle();
        estilo.fontSize = 40; // más grande
        estilo.normal.textColor = Color.red;
        estilo.alignment = TextAnchor.MiddleCenter; // centrado

        // posición en pantalla, un poco arriba del gato
        Vector3 posPantalla = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2);
        GUI.Label(new Rect(posPantalla.x - 50, Screen.height - posPantalla.y, 100, 40),
                  AtencionActual.ToString(), estilo);
    }

    void HandleMovement()
    {
        // si estoy trepando, no procesar movimiento normal
        if (isClimbing)
        {
            currentVelocity = Vector3.zero;
            velocity = Vector3.zero;
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(h, 0, v).normalized;
        Vector3 targetDirection = Vector3.zero;

        if (inputDir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            targetDirection = targetRotation * Vector3.forward;
        }

        if (targetDirection != Vector3.zero)
            currentVelocity = Vector3.MoveTowards(currentVelocity, targetDirection * speed, acceleration * Time.deltaTime);
        else
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);

        controller.Move(currentVelocity * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0) velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleCamera()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -30f, 60f);

        // Cambia la altura del pivot para que apunte al gato (ajusta 0.5f según tu modelo)
        Vector3 pivot = transform.position + Vector3.up * 0.5f;

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredCameraOffset = rotation * new Vector3(0, 0, -distanceFromPlayer);
        Vector3 desiredCameraPos = pivot + desiredCameraOffset;

        // Raycast para evitar atravesar paredes
        RaycastHit hit;
        float minDistance = 0.5f; // Distancia mínima para que la cámara no entre en el personaje
        Vector3 direction = (desiredCameraPos - pivot).normalized;
        float maxDistance = distanceFromPlayer;

        if (Physics.SphereCast(pivot, 0.2f, direction, out hit, maxDistance))
        {
            float hitDist = Mathf.Max(hit.distance - 0.1f, minDistance);
            cameraTransform.position = pivot + direction * hitDist;
        }
        else
        {
            cameraTransform.position = desiredCameraPos;
        }

        // Ahora la cámara mira al pivot más bajo (el gato)
        cameraTransform.LookAt(pivot);
    }

    void Saltar()
    {
        if (isClimbing) return; // no saltar mientras trepás

        if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
            velocity.y = Mathf.Sqrt(-2f * gravity * 1.5f);
    }

    void AgarrarCosas()
    {
        if (isClimbing) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hit, 3f))
            {
                Debug.Log("Agarraste: " + hit.collider.name);
                Interactuable interactuable = hit.collider.GetComponent<Interactuable>();
                if (interactuable != null)
                {
                    Vector3 direccionEmpuje = transform.forward;
                    interactuable.Interactuar(direccionEmpuje);
                }
            }
        }
    }

    void Trepar()
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
    public void RealizarDash()
    {
        Vector3 dashDir = (transform.position - Object.FindAnyObjectByType<Seguridad>().transform.position).normalized;
        controller.Move(dashDir * 5f); // Ajusta la fuerza del dash
    }

}
