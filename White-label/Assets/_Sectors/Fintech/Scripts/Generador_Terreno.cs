using System.Collections.Generic;
using UnityEngine;

public class GeneradorTerreno : MonoBehaviour
{
    [Header("Configuración de Bloques")]
    public GameObject[] bloquesPrefabs; // Aquí meteremos tus Bloque1, Bloque2, etc.
    public float longitudBloque = 100f; // La medida exacta de tu plano en el eje Z
    public int bloquesEnPantalla = 3;   // Cuántos bloques existen al mismo tiempo

    [Header("Referencias")]
    public Transform jugador;           // Para saber dónde está el personaje

    private float spawnZ = 0f;          // El punto donde aparecerá el siguiente bloque
    private float zonaSegura = 50f;     // Margen de distancia antes de borrar el bloque viejo
    private List<GameObject> bloquesActivos;

    void Start()
    {
        bloquesActivos = new List<GameObject>();

        // Al iniciar, generamos los primeros bloques de golpe para armar el piso inicial
        for (int i = 0; i < bloquesEnPantalla; i++)
        {
            GenerarBloque();
        }
    }

    void Update()
    {
        // 1. Calculamos dónde empezó el bloque más viejo que existe ahorita
        float inicioBloqueViejo = spawnZ - (bloquesEnPantalla * longitudBloque);
        
        // 2. Le decimos: "Si el jugador ya pasó el INICIO del bloque + toda su LONGITUD + la ZONA SEGURA, bórralo"
        if (jugador.position.z > inicioBloqueViejo + longitudBloque + zonaSegura)
        {
            GenerarBloque();
            BorrarBloqueViejo();
        }
    }

    void GenerarBloque()
    {
        // 1. Elegimos un bloque al azar de tu lista de Prefabs
        int indiceAleatorio = Random.Range(0, bloquesPrefabs.Length);
        
        // 2. Lo hacemos aparecer justo donde marca "spawnZ"
        GameObject nuevoBloque = Instantiate(bloquesPrefabs[indiceAleatorio], transform.forward * spawnZ, transform.rotation);
        
        // 3. Lo guardamos en la lista y movemos el punto de aparición más adelante
        bloquesActivos.Add(nuevoBloque);
        spawnZ += longitudBloque; 
    }

    void BorrarBloqueViejo()
    {
        // Destruimos el primer bloque de la lista (el que ya quedó atrás)
        Destroy(bloquesActivos[0]);
        bloquesActivos.RemoveAt(0);
    }
}