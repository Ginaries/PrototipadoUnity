using UnityEngine;

namespace ithappy.Animals_FREE
{
    [RequireComponent(typeof(CreatureMover))]
    public class MovePlayerInput_Player2 : MonoBehaviour
    {
        [Header("Character")]
        [SerializeField] private KeyCode upKey = KeyCode.I;
        [SerializeField] private KeyCode downKey = KeyCode.K;
        [SerializeField] private KeyCode leftKey = KeyCode.J;
        [SerializeField] private KeyCode rightKey = KeyCode.L;
        [SerializeField] private KeyCode runKey = KeyCode.Colon; // o KeyCode.Semicolon según layout
        [SerializeField] private KeyCode jumpKey = KeyCode.U; // opcional, si querés salto

        [Header("Camera")]
        [SerializeField] private PlayerCamera m_Camera;
        [SerializeField] private string m_MouseX = "Mouse X";
        [SerializeField] private string m_MouseY = "Mouse Y";
        [SerializeField] private string m_MouseScroll = "Mouse ScrollWheel";

        private CreatureMover m_Mover;

        private Vector2 m_Axis;
        private bool m_IsRun;
        private bool m_IsJump;
        private Vector3 m_Target;
        private Vector2 m_MouseDelta;
        private float m_Scroll;

        private void Awake()
        {
            m_Mover = GetComponent<CreatureMover>();
        }

        private void Update()
        {
            GatherInput();
            SetInput();
        }

        private void GatherInput()
        {
            // Movimiento con teclas individuales
            float horizontal = 0f;
            float vertical = 0f;

            if (Input.GetKey(leftKey)) horizontal = -1f;
            if (Input.GetKey(rightKey)) horizontal = 1f;
            if (Input.GetKey(upKey)) vertical = 1f;
            if (Input.GetKey(downKey)) vertical = -1f;

            m_Axis = new Vector2(horizontal, vertical).normalized;

            m_IsRun = Input.GetKey(runKey);
            m_IsJump = Input.GetKeyDown(jumpKey);

            // Cámara
            m_Target = (m_Camera == null) ? Vector3.zero : m_Camera.Target;
            m_MouseDelta = new Vector2(Input.GetAxis(m_MouseX), Input.GetAxis(m_MouseY));
            m_Scroll = Input.GetAxis(m_MouseScroll);
        }

        private void SetInput()
        {
            if (m_Mover != null)
                m_Mover.SetInput(in m_Axis, in m_Target, in m_IsRun, m_IsJump);

            if (m_Camera != null)
                m_Camera.SetInput(in m_MouseDelta, m_Scroll);
        }
    }
}
