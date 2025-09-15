using UnityEngine;

public class Trepable : MonoBehaviour
{
    [Tooltip("Punto exacto donde el player quedará después de trepar. Opcional")]
    public Transform climbPoint;

    public Vector3 GetPosicionTrepada()
    {
        if (climbPoint != null) return climbPoint.position;

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Vector3 top = col.bounds.center + Vector3.up * col.bounds.extents.y;
            return top + transform.forward * 0.5f; // un poco adelante del borde
        }

        return transform.position + Vector3.up * 1.5f;
    }
}
