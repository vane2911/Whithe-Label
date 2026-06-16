using UnityEngine;

public class Disco : MonoBehaviour
{
    [Header("Configuración de Marca")]
    public Texture2D logoCliente;
    public Color colorCorporativo = Color.cyan;

    [Header("Referencias de Materiales")]
    public int indiceMaterialLogo = 2; // Asegúrate de que siga en 4

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        Material[] materiales = renderer.materials;

        for (int i = 0; i < materiales.Length; i++)
        {
            // 1. Aplicar el Logo del Cliente
            if (i == indiceMaterialLogo && logoCliente != null)
            {
                // En URP la textura principal se llama _BaseMap
                materiales[i].SetTexture("_BaseMap", logoCliente);
            }

            // 2. Buscar TODOS los materiales que sean Neón (el 1 y el 3)
            if (materiales[i].name.Contains("Neon"))
            {
                // Forzamos a Unity a encender la propiedad de emisión
                materiales[i].EnableKeyword("_EMISSION"); 
                
                // Cambiamos el color de la pintura normal
                materiales[i].SetColor("_BaseColor", colorCorporativo); 
                
                // Cambiamos el color del brillo (el * 3f lo hace más intenso)
                materiales[i].SetColor("_EmissionColor", colorCorporativo * 3f); 
            }
        }
        
        // Devolvemos los materiales modificados al objeto
        renderer.materials = materiales;
    }
}