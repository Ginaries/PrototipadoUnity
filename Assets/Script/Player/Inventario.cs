using UnityEngine;
using System.Collections.Generic;

public class Inventario : MonoBehaviour
{
    public int monedas;
    public int llaves;

    public List<string> objetosRecogidos = new List<string>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (monedas < 0)
        {
            monedas = 0;
        }
        if (llaves < 0)
        {
            llaves = 0;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            monedas++;
        }
    }

    public void AgregarObjeto(string nombre)
    {
        objetosRecogidos.Add(nombre);
        Debug.Log("Recogiste: " + nombre);
    }
}