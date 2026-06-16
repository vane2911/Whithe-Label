using UnityEngine;

public class FlotarRecompensa : MonoBehaviour
{
    [Header("Ajustes Visuales")]
    public float amplitud = 0.2f; // Qué tanto sube y baja
    public float velocidadFlote = 2f; // Qué tan rápido flota
    public float velocidadRotacion = 30f; // Qué tan rápido gira sobre sí misma

    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.position;
    }

    void Update()
    {
        // 1. Efecto de flotar arriba y abajo
        float nuevaY = posicionInicial.y + Mathf.Sin(Time.time * velocidadFlote) * amplitud;
        transform.position = new Vector3(transform.position.x, nuevaY, transform.position.z);

        // 2. Efecto de girar lentamente para mostrarse
        transform.Rotate(Vector3.up, velocidadRotacion * Time.deltaTime);
    }
}