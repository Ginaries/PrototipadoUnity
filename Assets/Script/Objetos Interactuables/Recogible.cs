using UnityEngine;
using UnityEngine.UI;

public class Recogible : MonoBehaviour
{
    private Rigidbody rb;
    public string nombreObjeto = "Caja";
    public Text TextoAyuda; // Asigna el Text de interacción en el inspector

    private bool jugadorCerca = false;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();

    }
    void Start()
    {
        if (TextoAyuda == null)
        {
            GameObject go = GameObject.Find("TextInteraccion");
            if (go != null)
            {
                TextoAyuda = go.GetComponent<Text>();
                Debug.Log("✅ Encontrado TextInteraccion en Start");
            }
            else
            {
                Debug.LogWarning("⚠ No se encontró TextInteraccion en Start");
            }
        }
    }



    [System.Obsolete]
    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1.5f);
        bool playerDetectado = false;

        foreach (var col in colliders)
        {
            if (col.CompareTag("Player"))
            {
                playerDetectado = true;
                break;
            }
        }

        if (playerDetectado && !jugadorCerca)
        {
            jugadorCerca = true;
            cerca();
        }
        else if (!playerDetectado && jugadorCerca)
        {
            jugadorCerca = false;
            if (TextoAyuda != null)
                TextoAyuda.gameObject.SetActive(false);
        }

        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            // Agregar al inventario del jugador
            Inventario inventario = FindObjectOfType<Inventario>();
            if (inventario != null)
            {
                inventario.AgregarObjeto(nombreObjeto);
            }

            if (TextoAyuda != null)
                TextoAyuda.gameObject.SetActive(false);

            Destroy(gameObject); // Elimina el objeto de la escena
        }

    }
    void cerca()
    {
        if (TextoAyuda != null)
        {
            TextoAyuda.gameObject.SetActive(true);
            TextoAyuda.text = "Presiona 'E' para recoger " + nombreObjeto;
        }
    }
    // si es llave el objeto que envie una señal a la puerta para que se abra
    // referencia a la puerta
    [HeaderAttribute("Referencia a la puerta")]
    public PUERTA puerta;
    public void EsLlave()
    {
        if (puerta != null)
        {
            puerta.TieneLlave();
            Debug.Log("✅ La puerta ahora tiene la llave");
        }
        else
        {
            Debug.LogWarning("⚠ No se encontró una puerta en la escena");
        }
    }
    //cuando el objeto sea destruido llamar al metodo EsLlave si el objeto es una llave
    void OnDestroy()
    {
        if (nombreObjeto == "Llave")
        {
            EsLlave();
        }
    }
}