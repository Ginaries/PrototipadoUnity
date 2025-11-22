using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

[Serializable]
public struct DatosMision
{
    public string nombre;
    public float tiempo;
    public string estado;   // "completada" o "fallida"
}

public class MetricasJuego : MonoBehaviour
{
    private float tiempoInicioSesion;   // TIMER GLOBAL REAL
    private static int sesionID;

    private string rutaArchivo;

    private List<DatosMision> misiones = new List<DatosMision>();

    private int inputsCorrectos = 0;
    private int inputsIncorrectos = 0;

    // ----------------------------------------------------------
    // START — Inicia el timer global cuando arranca el gameplay
    // ----------------------------------------------------------
    void Start()
    {
        string escritorio = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string carpeta = Path.Combine(escritorio, "MetricasJuego");

        if (!Directory.Exists(carpeta))
            Directory.CreateDirectory(carpeta);

        rutaArchivo = Path.Combine(carpeta, "Metricas.txt");

        sesionID = ContarLineasArchivo(rutaArchivo) + 1;

        tiempoInicioSesion = Time.time; // TIMER GLOBAL

        Debug.Log($"Sesión iniciada (ID {sesionID}). Timer global inicializado.");
    }

    // ----------------------------------------------------------
    // MISIÓN COMPLETADA (USAMOS EL TIMER GLOBAL)
    // ----------------------------------------------------------
    public void CompletarMision(string nombre)
    {
        float tiempo = Time.time - tiempoInicioSesion;   // tiempo global desde el inicio

        misiones.Add(new DatosMision
        {
            nombre = nombre,
            tiempo = tiempo,
            estado = "completada"
        });

        Debug.Log($"Misión '{nombre}' completada en {tiempo:F2} segundos (timer global).");
    }

    // ----------------------------------------------------------
    // MISIÓN FALLIDA (USAMOS EL TIMER GLOBAL)
    // ----------------------------------------------------------
    public void FallarMision(string nombre)
    {
        float tiempo = Time.time - tiempoInicioSesion;

        misiones.Add(new DatosMision
        {
            nombre = nombre,
            tiempo = tiempo,
            estado = "fallida"
        });

        Debug.Log($"Misión '{nombre}' fallida en {tiempo:F2} segundos (timer global).");
    }

    // ----------------------------------------------------------
    //  REGISTRO DE INPUTS
    // ----------------------------------------------------------
    public void RegistrarInputCorrecto()
    {
        inputsCorrectos++;
    }

    public void RegistrarInputIncorrecto()
    {
        inputsIncorrectos++;
    }

    // ----------------------------------------------------------
    // GUARDAMOS AL SALIR
    // ----------------------------------------------------------
    private void OnApplicationQuit()
    {
        GuardarMetrica();
    }

    // ----------------------------------------------------------
    // GUARDADO
    // ----------------------------------------------------------
    private void GuardarMetrica()
    {
        using (StreamWriter writer = new StreamWriter(rutaArchivo, true))
        {
            writer.Write($"ID {sesionID}\t");

            foreach (var m in misiones)
            {
                TimeSpan ts = TimeSpan.FromSeconds(m.tiempo);
                string tiempoFormateado = $"{ts.Minutes:D2}:{ts.Seconds:D2}";
                writer.Write($"{m.nombre} ({m.estado}): {tiempoFormateado}\t");
            }

            writer.Write($"Inputs Correctos: {inputsCorrectos}\t");
            writer.Write($"Inputs Incorrectos: {inputsIncorrectos}\t");
            writer.WriteLine();
        }

        Debug.Log("Métrica guardada correctamente.");
    }

    // ----------------------------------------------------------
    // NUEVA SESIÓN
    // ----------------------------------------------------------
    public void GuardarYReiniciarSesion()
    {
        GuardarMetrica();

        sesionID++;
        misiones.Clear();
        inputsCorrectos = 0;
        inputsIncorrectos = 0;

        tiempoInicioSesion = Time.time;  // reset del timer global

        Debug.Log($"Nueva sesión iniciada con ID {sesionID}. Timer global reiniciado.");
    }

    private int ContarLineasArchivo(string ruta)
    {
        if (!File.Exists(ruta))
            return 0;

        return File.ReadAllLines(ruta).Length;
    }
}
