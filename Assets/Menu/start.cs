using UnityEngine;
using UnityEngine.SceneManagement;



public class start : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Jugar()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void Salir()
    {
        Application.Quit();

        // Esto es solo para que funcione dentro del editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}


