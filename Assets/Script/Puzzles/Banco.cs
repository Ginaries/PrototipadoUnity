using UnityEngine;

public class Banco : MonoBehaviour
{
    public int idBanco; // 1, 2, 3, etc.
    private ControlMision3 puzzleManager;

    [System.Obsolete]
    void Start()
    {
        // busca el puzzle manager en la escena
        puzzleManager = FindObjectOfType<ControlMision3>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // asegurate de que el gato tenga el tag Player
        {
            puzzleManager.RegistrarBanco(idBanco);
        }
    }
}
