using UnityEngine;
using UnityEngine.UI; // Importante para manejar ScrollRect y Buttons

public class Instrucciones : MonoBehaviour
{
    // --- REFERENCIAS ---
    public ScrollRect scrollPrincipal; // Tu componente Scroll Rect
    public float pasoDeDesplazamiento = 0.1f; // Qué tanto se mueve el scroll con cada clic (valor de 0 a 1)

    // --- FUNCIONES PÚBLICAS PARA LOS BOTONES ---

    public void SubirInstrucciones()
    {
        // La posición vertical del scroll va de 0 (abajo) a 1 (arriba).
        // Para subir, incrementamos el valor de normalizedPosition.
        float nuevaPosicion = scrollPrincipal.verticalNormalizedPosition + pasoDeDesplazamiento;
        
        // Usamos Clamp01 para asegurar que el valor no se salga del rango [0, 1]
        scrollPrincipal.verticalNormalizedPosition = Mathf.Clamp01(nuevaPosicion);
        
        Debug.Log("Subiendo. Nueva posición vertical: " + scrollPrincipal.verticalNormalizedPosition);
    }

    public void BajarInstrucciones()
    {
        // Para bajar, decrementamos el valor de normalizedPosition.
        float nuevaPosicion = scrollPrincipal.verticalNormalizedPosition - pasoDeDesplazamiento;
        
        scrollPrincipal.verticalNormalizedPosition = Mathf.Clamp01(nuevaPosicion);
        
        Debug.Log("Bajando. Nueva posición vertical: " + scrollPrincipal.verticalNormalizedPosition);
    }
}