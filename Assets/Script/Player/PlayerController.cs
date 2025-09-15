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
    public float distanceFromPlayer = 4f;
    public float cameraHeight = 2f;

    [Header("Trepar")]
    public float climbDistance = 1.8f;
    public float climbSpeed = 2f;
    private bool isClimbing = false;
    private Collider climbingCol;

    private CharacterController controller;
    private float yaw;
    private float pitch;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMovement();
        HandleCamera();
        Saltar();
        AgarrarCosas();
        Trepar();
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

        Vector3 targetPos = transform.position
                          - (Quaternion.Euler(0, yaw, 0) * Vector3.forward * distanceFromPlayer)
                          + Vector3.up * cameraHeight;

        cameraTransform.position = targetPos;
        cameraTransform.LookAt(transform.position + Vector3.up * 1.2f);
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
}
