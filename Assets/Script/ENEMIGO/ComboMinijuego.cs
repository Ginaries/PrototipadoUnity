using UnityEngine;
using UnityEngine.UI;

public class ComboMinijuego : MonoBehaviour
{
    public bool EstaActivo() => activo;
    public GameObject canvasCombo;
    public Text comboText;
    public float tiempoPorTecla = 3f;

    private string[] teclas = { "A", "D", "W", "S" };
    private string[] comboActual = new string[3];
    private int indice = 0;
    private float tiempoRestante;
    private bool activo = false;
    private bool fallo = false;
    private PlayerController player;

    // Update is called once per frame
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
                    // Dash exitoso
                    player.RealizarDash();
                    Desactivar();
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
        canvasCombo.SetActive(true);

        // Genera una secuencia de 3 teclas aleatorias
        for (int i = 0; i < comboActual.Length; i++)
        {
            comboActual[i] = teclas[Random.Range(0, teclas.Length)];
        }
        indice = 0;
        fallo = false;
        SiguienteTecla();
        activo = true;
        Time.timeScale = 0.2f; // efecto de cámara lenta opcional
    }

    void SiguienteTecla()
    {
        KeyCode teclaActual = (KeyCode)System.Enum.Parse(typeof(KeyCode), comboActual[indice]);
        comboText.text = "Presiona: " + teclaActual;
        tiempoRestante = tiempoPorTecla; // siempre resetea el contador
    }


    void MostrarPerdiste()
    {
        comboText.text = "¡Perdiste!";
        fallo = true;
        Invoke(nameof(Desactivar), 1.5f); // Espera un poco antes de cerrar
    }

    public void Desactivar()
    {
        canvasCombo.SetActive(false);
        activo = false;
        Time.timeScale = 1f;
    }
}
