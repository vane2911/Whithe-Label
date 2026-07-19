using UnityEngine;

public class FijarBlanco : MonoBehaviour
{
    [Header("Estado")]
    public bool activo = false;
    public Transform objetivo;

    [Header("Configuración")]
    [Tooltip("Qué tan rápido gira el personaje hacia el costal")]
    public float velocidadGiro = 15f;

    // LateUpdate ocurre DESPUÉS de que el StarterAssets movió al personaje
    private void LateUpdate()
    {
        if (activo && objetivo != null)
        {
            // 1. Calculamos la dirección hacia el costal
            Vector3 direccionHaciaCostal = objetivo.position - transform.position;
            
            // 2. Anulamos la altura (Y) para que el personaje no se incline hacia el piso o el techo
            direccionHaciaCostal.y = 0f;

            // 3. Si estamos muy cerca y el vector es casi cero, evitamos errores matemáticos
            if (direccionHaciaCostal.sqrMagnitude > 0.01f)
            {
                // 4. Forzamos la rotación suavemente hacia el costal
                Quaternion rotacionDeseada = Quaternion.LookRotation(direccionHaciaCostal);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, Time.deltaTime * velocidadGiro);
            }
        }
    }
}