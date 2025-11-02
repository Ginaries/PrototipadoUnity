using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class ComboMinijuego : MonoBehaviour
{
    public bool EstaActivo() => activo;

    [Header("UI")]
    public GameObject panelCombo;
    public TextMeshProUGUI comboText;

    [Header("Configuración General")]
    public float tiempoPorTecla = 3f;

    [Header("Mini botones estilo Osu!")]
    public GameObject botonPrefab;
    public RectTransform botonSpawnArea;
    public float intervaloSpawnBoton = 1.5f;

    private string[] teclas = { "Q", "E", "Alpha1", "Alpha2", "Alpha3", "Space", "Z", "X", "C" };
    private string teclaActual;
    private float tiempoRestante;
    private bool activo = false;
    private PlayerController player;
    private bool modoTeclas; // true = teclas, false = botones

    public System.Action<bool> OnComboTerminado;

    [Header("UI_FEEDBACK")]
    public GameObject CartelDistraccion;

    private List<GameObject> botonesActivos = new List<GameObject>();
    private bool spawneando = false;

    void Start()
    {
        panelCombo.SetActive(false);
        comboText?.gameObject.SetActive(false);
        CartelDistraccion.SetActive(false);

    }

    void Update()
    {
        if (!activo) return;

        tiempoRestante -= Time.deltaTime;

        // Si el jugador ya tiene la atención al 100%, terminamos todo
        if (player.AtencionActual >= player.AtencionMax)
        {
            Desactivar();
            OnComboTerminado?.Invoke(true);
            return;
        }

        // Si está en modo teclas
        if (modoTeclas)
        {
            if (Input.anyKeyDown)
            {
                KeyCode tecla = (KeyCode)System.Enum.Parse(typeof(KeyCode), teclaActual);

                if (Input.GetKeyDown(tecla))
                {
                    player.AumentarAtencion();
                    FindAnyObjectByType<MetricasJuego>().RegistrarInputCorrecto();
                    NuevaRonda();
                }
                else
                {
                    FindAnyObjectByType<MetricasJuego>().RegistrarInputIncorrecto();
                    NuevaRonda();
                }
            }

            if (tiempoRestante <= 0)
            {
                NuevaRonda();
            }
        }
    }

    public void Activar(PlayerController p)
    {
        player = p;
        panelCombo.SetActive(true);
        comboText?.gameObject.SetActive(true);
        CartelDistraccion.SetActive(true);

        // Mostrar cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        activo = true;
        NuevaRonda();
    }

    void NuevaRonda()
    {
        Debug.Log("Nueva ronda iniciada.");

        if (comboText == null)
            Debug.LogError("comboText no está asignado en el inspector!");
        // Si ya está completo, termina
        if (player.AtencionActual >= player.AtencionMax)
        {
            Desactivar();
            return;
        }

        // Limpia botones viejos si quedaron
        LimpiarBotones();

        // Decide si toca botones o teclas
        modoTeclas = Random.value < 0.5f;

        if (modoTeclas)
        {
            teclaActual = teclas[Random.Range(0, teclas.Length)];
            string teclaMostrar = teclaActual.Replace("Alpha", "");
            comboText.text = "Presiona: " + teclaMostrar;
            tiempoRestante = tiempoPorTecla;
        }

        else
        {
            comboText.text = "¡Haz click en el círculo!";
            if (!spawneando) StartCoroutine(SpawnBotonUnico());
        }
    }

    IEnumerator SpawnBotonUnico()
    {
        spawneando = true;

        Vector2 randomPos = new Vector2(
            Random.Range(-botonSpawnArea.rect.width / 2f, botonSpawnArea.rect.width / 2f),
            Random.Range(-botonSpawnArea.rect.height / 2f, botonSpawnArea.rect.height / 2f)
        );

        GameObject boton = Instantiate(botonPrefab, botonSpawnArea);
        boton.transform.localPosition = randomPos;
        boton.transform.localScale = Vector3.one;
        boton.SetActive(true);
        botonesActivos.Add(boton);

        Button btn = boton.GetComponent<Button>();
        bool fuePresionado = false;

        btn.onClick.AddListener(() =>
        {
            if (!fuePresionado)
            {
                fuePresionado = true;
                player.AumentarAtencion();
                FindAnyObjectByType<MetricasJuego>().RegistrarInputCorrecto();
                Destroy(boton);
                botonesActivos.Remove(boton);
                spawneando = false;
                NuevaRonda();
            }
        });

        yield return new WaitForSeconds(1.5f);

        if (!fuePresionado)
        {
            FindAnyObjectByType<MetricasJuego>().RegistrarInputIncorrecto();
            Destroy(boton);
            botonesActivos.Remove(boton);
            spawneando = false;
            NuevaRonda();
        }
    }

    public void Desactivar()
    {
        activo = false;
        StopAllCoroutines();
        LimpiarBotones();

        comboText?.gameObject.SetActive(false);
        panelCombo.SetActive(false);

        // Ocultar cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (player != null)
        {
            player.DistraccionActiva = false;
            CartelDistraccion.SetActive(false);
            player.ActualizarBarraAtencion();
        }
    }

    void LimpiarBotones()
    {
        foreach (var boton in botonesActivos)
        {
            if (boton != null) Destroy(boton);
        }
        botonesActivos.Clear();
        spawneando = false;
    }
}
