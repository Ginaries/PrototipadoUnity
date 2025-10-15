using UnityEngine;
using UnityEngine.UI;

public class ComboMinijuego : MonoBehaviour
{
    public bool EstaActivo() => activo;

    [Header("UI")]
    public Text comboText;
    public Text textoEXCELENTE;

    [Header("Configuración")]
    public float tiempoPorTecla = 3f;

    private string[] teclas = { "Q", "E", "Alpha1", "Alpha2", "Alpha3", "Space", "Z", "X", "C" };
    private string[] comboActual = new string[3];
    private int indice = 0;
    private float tiempoRestante;
    private bool activo = false;
    private PlayerController player;

    public System.Action<bool> OnComboTerminado;

    [Header("UI_FEEDBACK")]
    public GameObject CartelDistraccion;
    void Start()
    {
        if (comboText != null)
            comboText.gameObject.SetActive(false);
        CartelDistraccion.SetActive(false);
    }

    void Update()
    {
        if (!activo) return;

        tiempoRestante -= Time.unscaledDeltaTime;

        if (Input.anyKeyDown)
        {
            KeyCode teclaActual = (KeyCode)System.Enum.Parse(typeof(KeyCode), comboActual[indice]);

            if (Input.GetKeyDown(teclaActual))
            {
                // ✅ Tecla correcta
                player.AumentarAtencion(); // usa el método original
                textoEXCELENTE.text = "¡EXCELENTE!";

                // 🔹 Si ya tiene la atención al máximo, se termina el minijuego
                if (player.AtencionActual >= player.AtencionMax)
                {
                    textoEXCELENTE.text = "¡Atención completa!";
                    Desactivar();
                    OnComboTerminado?.Invoke(true);
                    return;
                }

                indice++;

                if (indice >= comboActual.Length)
                {
                    // genera un nuevo combo
                    Activar(player);
                    return;
                }

                SiguienteTecla();
            }
            else
            {
                // 🔸 tecla incorrecta → solo mensaje, no se pierde
                textoEXCELENTE.text = "Fallo...";
            }
        }

        if (tiempoRestante <= 0)
        {
            // ⏰ si el tiempo se acaba, se pasa a un nuevo combo
            textoEXCELENTE.text = "Tarde...";
            Activar(player);
        }
    }

    public void Activar(PlayerController p)
    {

        player = p;
        if (comboText != null)
            comboText.gameObject.SetActive(true);
        CartelDistraccion.SetActive(true);

        // genera 3 teclas aleatorias
        for (int i = 0; i < comboActual.Length; i++)
        {
            comboActual[i] = teclas[Random.Range(0, teclas.Length)];
        }

        indice = 0;
        SiguienteTecla();
        activo = true;
        Time.timeScale = 0.2f;
    }

    void SiguienteTecla()
    {
        KeyCode teclaActual = (KeyCode)System.Enum.Parse(typeof(KeyCode), comboActual[indice]);
        comboText.text = "Presiona: " + teclaActual;
        tiempoRestante = tiempoPorTecla;
    }

    public void Desactivar()
    {
        if (comboText != null)
            comboText.gameObject.SetActive(false);

        activo = false;
        Time.timeScale = 1f;

        if (player != null)
        {
            player.DistraccionActiva = false;
            CartelDistraccion.SetActive(false);
            player.ActualizarBarraAtencion();
        }
    }
}
