using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class Inventario : MonoBehaviour
{
    public int Monedas;
    public int Llaves;


    public List<string> objetosRecogidos = new List<string>();


    [Header("UI")]
    public GameObject CartelLlave; // 👈 arrastrá el PNG aquí en el Inspector


    void Start()
    {
        if (CartelLlave != null)
            CartelLlave.SetActive(false); // aseguramos que arranque oculto
    }


    void Update()
    {
        if (Monedas < 0)
        {
            Monedas = 0;
        }
        if (Llaves < 0)
        {
            Llaves = 0;
        }


        // Solo para prueba: apretar M suma monedas
        if (Input.GetKeyDown(KeyCode.M))
        {
            Monedas++;
        }
    }


 public void AgregarObjeto(string nombre)
{
    objetosRecogidos.Add(nombre);
    Debug.Log("Recogiste: " + nombre);


    if (nombre == "Llave")
    {
        Debug.Log("✅ Se activó el cartel de Llave"); // 👈 prueba visual
        Llaves++;
        MostrarCartelLlave();
    }
    else if (nombre == "moneda")
    {
        Monedas++;
    }
}


    void MostrarCartelLlave()
    {
        if (CartelLlave != null)
            StartCoroutine(MostrarCartelPorTiempo(1f)); // aparece el cartel por 1 segundo
    }


    IEnumerator MostrarCartelPorTiempo(float duracion)
    {
        CartelLlave.SetActive(true);
        yield return new WaitForSeconds(duracion);
        CartelLlave.SetActive(false);
    }
}
