using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;
    public float rotationSpeed = 10f; // más bajo = más suave
    public float gravity = -9.81f;  // 👈 fuerza de gravedad
    private Vector3 velocity;       // 👈 acumula gravedad

    [Header("Cámara")]
    public Transform cameraTransform; // Cámara que sigue al player
    public float mouseSensitivity = 3f;
    public float distanceFromPlayer = 4f;
    public float cameraHeight = 2f;

    private CharacterController controller;
    private float yaw;   // rotación horizontal
    private float pitch; // rotación vertical

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // oculta y bloquea el cursor
    }

    void Update()
    {
        HandleMovement();
        HandleCamera();
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(h, 0, v).normalized;

        if (inputDir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            Vector3 moveDir = targetRotation * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        // 👇 aplicar gravedad
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // para mantener pegado al piso
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleCamera()
    {
        // movimiento del mouse
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -30f, 60f); // limitar cámara arriba/abajo

        // posición de cámara
        Vector3 targetPos = transform.position
                          - (Quaternion.Euler(0, yaw, 0) * Vector3.forward * distanceFromPlayer)
                          + Vector3.up * cameraHeight;

        cameraTransform.position = targetPos;
        cameraTransform.LookAt(transform.position + Vector3.up * 1.2f); // mira al player
    }
}
