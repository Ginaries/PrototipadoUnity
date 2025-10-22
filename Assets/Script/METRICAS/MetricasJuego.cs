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

    void Awake()
    {
        // 📁 Carpeta de métricas en el Escritorio
        string escritorio = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string carpeta = Path.Combine(escritorio, "MetricasJuego");

        if (!Directory.Exists(carpeta))
            Directory.CreateDirectory(carpeta);

        rutaArchivo = Path.Combine(carpeta, "Metricas.txt");

        // Generar ID de sesión
        sesionID = ContarLineasArchivo(rutaArchivo) + 1;

        tiempoInicio = Time.time;
        Debug.Log($"Metrica iniciada (ID {sesionID}). Guardando en: {rutaArchivo}");
    }

    public void RegistrarMision(string nombreMision)
    {
        float tiempoActual = Time.time - tiempoInicio;
        tiemposMisiones[nombreMision] = tiempoActual;

        Debug.Log($"{nombreMision} completada en {tiempoActual:F2} segundos.");
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

            foreach (var mision in tiemposMisiones)
            {
                TimeSpan ts = TimeSpan.FromSeconds(mision.Value);
                string tiempoFormateado = $"{ts.Minutes:D2}:{ts.Seconds:D2}";
                writer.Write($"{mision.Key}: {tiempoFormateado}\t");
            }

            writer.WriteLine(); // salto de línea
        }

        Debug.Log("Métrica guardada correctamente en el Escritorio.");
    }

    private int ContarLineasArchivo(string ruta)
    {
        if (!File.Exists(ruta)) return 0;
        return File.ReadAllLines(ruta).Length;
    }
}
