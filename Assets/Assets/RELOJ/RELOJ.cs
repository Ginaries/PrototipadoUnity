using UnityEngine;

public class RELOJ : MonoBehaviour
{
    public Sprite spriteFrente;

    public float velocidadRotacion = 90f; // grados por segundo

    private SpriteRenderer sr;
    private float rotacionY = 0f;
    private bool mostrandoFrente = true;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = spriteFrente;
    }

    void Update()
    {
        // Rotar en Y
        rotacionY += velocidadRotacion * Time.deltaTime;

        // Aplicar rotación visual
        transform.rotation = Quaternion.Euler(0, rotacionY, 0);

        // Cuando da una vuelta completa (360 grados)
        if (rotacionY >= 360.0f)
        {
            rotacionY = 0f;

        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && mostrandoFrente)
        {
            //Spawn de objetos de minijuego
            
        }
    }
}
