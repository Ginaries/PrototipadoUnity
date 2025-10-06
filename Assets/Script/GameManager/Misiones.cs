using UnityEngine;
using TMPro; // Si usas TextMeshPro

public class Misiones : MonoBehaviour
{
    public static Misiones Instance;

    [Header("Referencias UI")]
    public TMP_Text textoMision; // Asignar desde el inspector

    [Header("Lista de misiones")]
    [TextArea(2, 5)]
    public string[] misiones = {
        "Misión 1: Encuentra la llave perdida.",
        "Misión 2: Interactua con el Guardia",
    };

    public int misionActual = 0;

    private void Awake()
    {
        // --- Singleton ---
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // No se destruye al cambiar de escena
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        ActualizarTextoMision();
    }

    public void CompletarMision()
    {
        if (misionActual < misiones.Length - 1)
        {
            misionActual++;
            ActualizarTextoMision();
        }
        else
        {
            textoMision.text = "🎉 ¡Todas las misiones completadas!";
        }
    }

    public void ActualizarTextoMision()
    {
        if (textoMision != null)
            textoMision.text = misiones[misionActual];
    }

    public string GetMisionActual()
    {
        return misiones[misionActual];
    }
}
