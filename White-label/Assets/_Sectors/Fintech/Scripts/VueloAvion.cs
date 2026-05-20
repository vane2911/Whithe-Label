using UnityEngine;

public class VueloAvion : MonoBehaviour
{
    public float velocidadAvion = 15f; 

    void Update()
    {
        // Usamos Space.World para garantizar que se mueva hacia el Norte del mundo (eje Z),
        // ¡exactamente igual que tu personaje!
        Vector3 direccionAvion = new Vector3(0, 0, velocidadAvion);
        transform.Translate(direccionAvion * Time.deltaTime, Space.World);
    }
}