using UnityEngine;

public class Puntos : MonoBehaviour
{
    public int valorPuntos = 10; // Así el cliente puede decidir si esta estrella vale 10 o 50

    void OnTriggerEnter(Collider otro)
    {
        Debug.Log("¡Algo tocó la estrella! Fue: " + otro.gameObject.name);
        // Solo reaccionamos si el jugador nos toca
        if (otro.CompareTag("Player")) 
        {
            // Buscamos al "Cerebro" (GestorJuego) para sumar los puntos
            GestorJuego gestor = FindFirstObjectByType<GestorJuego>();
            if (gestor != null)
            {
                gestor.SumarPuntos(valorPuntos);
            }
            
            // Destruimos el objeto recolectado
            Destroy(gameObject);
        }
    }
}