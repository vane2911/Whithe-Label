using UnityEngine;

public class GeneradorAnillos : MonoBehaviour
{
    public GameObject anilloPrefab; // El "molde" de tu anillo
    public Transform jugador;       // Para saber dónde estás y poner el anillo frente a ti
    
    public float tiempoEntreAnillos = 2f;  // Cada cuántos segundos aparece un anillo
    public float distanciaAdelante = 150f; // Qué tan lejos (en el eje Z) aparece frente a ti
    public float rangoLateral = 20f;       // Para que aparezcan un poco a la izquierda o derecha al azar
    
    private float temporizador = 0f;

    void Update()
    {
        // El reloj avanza
        temporizador += Time.deltaTime;

        // Cuando el reloj llega al límite (ej. 2 segundos), creamos un anillo
        if (temporizador >= tiempoEntreAnillos)
        {
            CrearAnillo();
            temporizador = 0f; // Reiniciamos el reloj a 0
        }
    }

    void CrearAnillo()
    {
        // 1. Elegimos una posición al azar hacia la izquierda o derecha
        float posicionXRandom = Random.Range(-rangoLateral, rangoLateral);
        
        // 2. Calculamos dónde aparecerá: a tu altura (Y), pero muy adelante (Z) y movido a los lados (X)
        Vector3 posicionAparicion = new Vector3(
            jugador.position.x + posicionXRandom, 
            jugador.position.y, 
            jugador.position.z + distanciaAdelante
        );

        // 3. ¡Puf! Hacemos aparecer el anillo usando el molde
        Instantiate(anilloPrefab, posicionAparicion, Quaternion.identity);
    }
}