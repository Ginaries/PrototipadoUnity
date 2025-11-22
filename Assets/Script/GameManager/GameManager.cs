using UnityEngine;
using UnityEngine.SceneManagement;
public class ExitGame : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Menu");
            FindAnyObjectByType<MetricasJuego>().GuardarYReiniciarSesion();
        }
    }
}
