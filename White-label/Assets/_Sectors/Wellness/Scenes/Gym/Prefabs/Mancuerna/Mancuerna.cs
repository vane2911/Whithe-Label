using UnityEngine;

public class Mancuerna : MonoBehaviour
{
    [Header("Configuración de Marca")]
    public Texture2D logoCliente;
    public Color colorCorporativo = Color.cyan;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        Material[] materiales = renderer.materials;

        for (int i = 0; i < materiales.Length; i++)
        {
            // 1. Aplicar el Logo del Cliente
            // En lugar de un índice fijo, buscamos el material que tú nombraste para el logo
            if (materiales[i].name.Contains("Logo") && logoCliente != null)
            {
                materiales[i].SetTexture("_BaseMap", logoCliente);
            }

            // 2. Configurar los materiales Neón
            if (materiales[i].name.Contains("Neon"))
            {
                materiales[i].EnableKeyword("_EMISSION"); 
                materiales[i].SetColor("_BaseColor", colorCorporativo); 
                materiales[i].SetColor("_EmissionColor", colorCorporativo * 3f); 
            }
        }
        
        renderer.materials = materiales;
    }
}