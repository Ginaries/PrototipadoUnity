using UnityEngine;

public class PUERTA : MonoBehaviour
{
    //si el jugador tiene la llave
    public bool tieneLlave = false;
    //el objeto llave
    public GameObject llave;
    //si la puerta esta abierta
    public bool puertaAbierta = false;
    //abrir la puerta a traves de rotar el objeto
    public void AbrirPuerta()
    {
        if (tieneLlave && !puertaAbierta)
        {
            //rotarla 90 grados en el eje y lentamente hasta llegar a los 90 grados
            float rotacionActual = transform.rotation.eulerAngles.y;
            float rotacionObjetivo = rotacionActual + 90f;
            float velocidadRotacion = 10f; // Velocidad de rotación
            float tiempoTranscurrido = 0f;
            while (tiempoTranscurrido < 1f)
            {
                tiempoTranscurrido += Time.deltaTime * velocidadRotacion;
                float nuevaRotacion = Mathf.Lerp(rotacionActual, rotacionObjetivo, tiempoTranscurrido);
                transform.rotation = Quaternion.Euler(0f, nuevaRotacion, 0f);
            }
            

        }
    }
    //hacer el metodo tiene llave publico para que pueda ser llamado desde otro script
    public void TieneLlave()
    {
        tieneLlave = true;
        AbrirPuerta();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //ejecutar una prueba para abrir la puerta
        //tieneLlave = true;
        //AbrirPuerta();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
