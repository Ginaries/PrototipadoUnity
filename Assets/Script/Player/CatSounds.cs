using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CatSounds : MonoBehaviour
{
    public AudioSource audioSource;   // El AudioSource del gato
    public AudioClip meowClip;        // Clip del maullido
    public float minInterval = 5f;    // Tiempo mínimo entre maullidos
    public float maxInterval = 15f;   // Tiempo máximo entre maullidos

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        StartCoroutine(RandomMeow());
    }

    private System.Collections.IEnumerator RandomMeow()
    {
        while (true)
        {
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            audioSource.PlayOneShot(meowClip);
        }
    }
}
