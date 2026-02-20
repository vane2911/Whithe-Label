using UnityEngine;

public class FuenteAgua : MonoBehaviour, IInteractable
{
    private Renderer myRenderer;
    private Color originalEmission;
    
    [Header("Ajustes de Brillo")]
    [ColorUsage(true, true)] 
    public Color highlightColor = Color.white; 

    void Start()
    {
        myRenderer = GetComponentInChildren<Renderer>();
        if (myRenderer != null)
        {
            // Guardamos el color inicial (que debe ser negro)
            originalEmission = myRenderer.material.GetColor("_EmissionColor");
        }
    }

    public void Interact()
    {
        Debug.Log("Mich, el personaje está bebiendo agua de la fuente.");
    }

    public void OnFocus()
    {
        if (myRenderer != null)
        {
            myRenderer.material.SetColor("_EmissionColor", highlightColor);
            myRenderer.material.EnableKeyword("_EMISSION");
        }
    }

    public void OnLoseFocus()
    {
        if (myRenderer != null)
        {
            myRenderer.material.SetColor("_EmissionColor", originalEmission);
        }
    }
}