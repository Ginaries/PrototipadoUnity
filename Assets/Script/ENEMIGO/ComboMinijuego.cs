using UnityEngine;
using UnityEngine.UI;

public class ComboMinijuego : MonoBehaviour
{
    public GameObject canvasCombo;
    public Text comboText;
    public float tiempoMax = 2f;

    private string[] teclas = { "A", "D", "W", "S" };
    private string comboActual = "";
    private int indice = 0;
    private float tiempoRestante;
    private bool activo = false;
    private PlayerController player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!activo) return;

        tiempoRestante -= Time.unscaledDeltaTime;
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(comboActual.ToLower()))
            {
                // Dash exitoso
                player.RealizarDash();
                Desactivar();
            }
            else
            {
                // Falló el combo
                Desactivar();
            }
        }

        if (tiempoRestante <= 0)
        {
            Desactivar();
        }
    }

    public void Activar(PlayerController p)
    {
        player = p;
        canvasCombo.SetActive(true);
        comboActual = teclas[Random.Range(0, teclas.Length)];
        comboText.text = comboActual;
        indice = 0;
        tiempoRestante = tiempoMax;
        activo = true;
        Time.timeScale = 0.2f; // efecto de cámara lenta opcional
    }

    public void Desactivar()
    {
        canvasCombo.SetActive(false);
        activo = false;
        Time.timeScale = 1f;
    }
}
