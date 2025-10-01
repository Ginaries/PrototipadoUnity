using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    private bool fallo = false;
    private PlayerController player;

    // 🔹 NUEVO: evento que avisa al guardia si el combo terminó
    public System.Action<bool> OnComboTerminado;

    void Start()
    {
        if (comboText != null)
            comboText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!activo) return;

        tiempoRestante -= Time.unscaledDeltaTime;

        if (!fallo && Input.anyKeyDown)
        {
            KeyCode teclaActual = (KeyCode)System.Enum.Parse(typeof(KeyCode), comboActual[indice]);

            if (Input.GetKeyDown(teclaActual))
            {
                indice++;
                if (indice >= comboActual.Length)
                {
                    // ✅ Combo exitoso
                    textoEXCELENTE.text = "¡EXCELENTE!";
                    player.AumentarAtencion();
                    Desactivar();

                    // 🔹 Avisar al guardia que el combo terminó con éxito
                    OnComboTerminado?.Invoke(true);
                    return;
                }
                else
                {
                    SiguienteTecla();
                }
            }
            else
            {
                MostrarPerdiste();
            }
        }

        if (!fallo && tiempoRestante <= 0)
        {
            MostrarPerdiste();
        }
    }

    public void Activar(PlayerController p)
    {
        player = p;
        if (comboText != null)
            comboText.gameObject.SetActive(true);

        // genera 3 teclas aleatorias
        for (int i = 0; i < comboActual.Length; i++)
        {
            comboActual[i] = teclas[Random.Range(0, teclas.Length)];
        }

        indice = 0;
        fallo = false;
        SiguienteTecla();
        activo = true;
        Time.timeScale = 0.2f; // cámara lenta opcional
    }

    void SiguienteTecla()
    {
        KeyCode teclaActual = (KeyCode)System.Enum.Parse(typeof(KeyCode), comboActual[indice]);
        comboText.text = "Presiona: " + teclaActual;
        tiempoRestante = tiempoPorTecla;
    }

    void MostrarPerdiste()
    {
        textoEXCELENTE.text = "";
        comboText.text = "¡Perdiste!";
        fallo = true;

        // 🔹 Avisar al guardia que el combo terminó (fallo = false)
        OnComboTerminado?.Invoke(false);

        Invoke(nameof(ReiniciarNivel), 1.5f);
    }

    public void ReiniciarNivel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
            player.ActualizarBarraAtencion();
        }
    }
}
