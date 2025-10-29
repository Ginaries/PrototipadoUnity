using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public class MetricasJuego : MonoBehaviour
{
    private float tiempoInicio;
    private Dictionary<string, float> tiemposMisiones = new Dictionary<string, float>();
    private string rutaArchivo;
    private static int sesionID;

    // --- NUEVOS CONTADORES ---
    private int inputsCorrectos = 0;
    private int inputsIncorrectos = 0;

    void Awake()
    {
        string escritorio = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string carpeta = Path.Combine(escritorio, "MetricasJuego");

        if (!Directory.Exists(carpeta))
            Directory.CreateDirectory(carpeta);

        rutaArchivo = Path.Combine(carpeta, "Metricas.txt");

        // Generar ID de sesión (1 por línea)
        sesionID = ContarLineasArchivo(rutaArchivo) + 1;

        tiempoInicio = Time.time;
        Debug.Log($"Métrica iniciada (ID {sesionID}). Guardando en: {rutaArchivo}");
    }

    // --- Registrar misión completada ---
    public void RegistrarMision(string nombreMision)
    {
        float tiempoActual = Time.time - tiempoInicio;
        tiemposMisiones[nombreMision] = tiempoActual;

        Debug.Log($"{nombreMision} completada en {tiempoActual:F2} segundos.");
    }
    // --- Registrar input correcto ---
    public void RegistrarInputCorrecto()
    {
        inputsCorrectos++;
        Debug.Log($"Input correcto registrado. Total: {inputsCorrectos}");
    }

    // --- Registrar input incorrecto ---
    public void RegistrarInputIncorrecto()
    {
        inputsIncorrectos++;
        Debug.Log($"Input incorrecto registrado. Total: {inputsIncorrectos}");
    }

    private void OnApplicationQuit()
    {
        GuardarMetrica();
    }

    private void GuardarMetrica()
    {
        using (StreamWriter writer = new StreamWriter(rutaArchivo, true))
        {
            writer.Write($"ID {sesionID}\t");

            // Escribir tiempos de misiones
            foreach (var mision in tiemposMisiones)
            {
                TimeSpan ts = TimeSpan.FromSeconds(mision.Value);
                string tiempoFormateado = $"{ts.Minutes:D2}:{ts.Seconds:D2}";
                writer.Write($"{mision.Key}: {tiempoFormateado}\t");
            }

            // Agregar los inputs correctos e incorrectos
            writer.Write($"inputs minijuegos Correctos: {inputsCorrectos}\t");
            writer.Write($"inputs minijuegos Incorrectos: {inputsIncorrectos}\t");

            writer.WriteLine(); // salto de línea (una línea por sesión)
        }

        Debug.Log("Métrica guardada correctamente en el Escritorio.");
    }

    private int ContarLineasArchivo(string ruta)
    {
        if (!File.Exists(ruta)) return 0;
        return File.ReadAllLines(ruta).Length;
    }
}
