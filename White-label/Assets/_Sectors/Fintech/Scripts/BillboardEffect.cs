using UnityEngine;

public class BillboardEffect : MonoBehaviour
{
    private Transform mainCameraTransform;

    void Start()
    {
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        if (mainCameraTransform != null)
        {
            // Calculamos hacia dónde mirar (manteniendo la altura)
            Vector3 targetPosition = mainCameraTransform.position;
            targetPosition.y = transform.position.y; 
            
            transform.LookAt(targetPosition);

            // === LA SOLUCIÓN ESTÁ AQUÍ ===
            // Quitamos las barras '//' de la línea de abajo para activarla:
            transform.Rotate(0, 180, 0); 
            // =============================
        }
    }
}