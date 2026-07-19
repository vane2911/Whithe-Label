using UnityEngine;

public class Puntos : MonoBehaviour
{
    public int valorPuntos = 10; 

    void OnTriggerEnter(Collider otro)
    {
        Debug.Log("¡Algo tocó la estrella! Fue: " + otro.gameObject.name);
        if (otro.CompareTag("Player")) 
        {
            GestorJuego gestor = FindFirstObjectByType<GestorJuego>();
            if (gestor != null)
            {
                gestor.SumarPuntos(valorPuntos);
                gestor.ReproducirSonidoMoneda();
            }
            
            Destroy(gameObject);
        }
    }
}