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
        // Crear carpeta y archivo si no existen
        string carpeta = Application.persistentDataPath + "/Metricas";
        if (!Directory.Exists(carpeta))
            Directory.CreateDirectory(carpeta);

        rutaArchivo = Path.Combine(carpeta, "Metricas.txt");

        // Leer cuántas sesiones hay para generar el nuevo ID
        sesionID = ContarLineasArchivo(rutaArchivo) + 1;

        tiempoInicio = Time.time;
        Debug.Log("Metrica iniciada. ID de sesión: " + sesionID);
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

        Debug.Log("Métrica guardada correctamente en: " + rutaArchivo);
    }

    private int ContarLineasArchivo(string ruta)
    {
        if (!File.Exists(ruta)) return 0;
        return File.ReadAllLines(ruta).Length;
    }
}
