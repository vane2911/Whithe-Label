using UnityEngine;
 
public class FuenteAgua : MonoBehaviour, IInteractable
{
    private Renderer myRenderer;
    private Color originalEmission;
 
    [Header("Ajustes de Brillo")]
    [ColorUsage(true, true)]
    public Color highlightColor = Color.white;
 
    [Header("WhiteLabel - Texto de Interacción")]
    [Tooltip("Texto que se muestra cuando el jugador apunta a la fuente")]
    [SerializeField] private string textoInteraccion;
 
    void Start()
    {
        myRenderer = GetComponentInChildren<Renderer>();
        if (myRenderer != null)
        {
            originalEmission = myRenderer.material.GetColor("_EmissionColor");
        }
    }
 
    public void Interact()
    {
        Debug.Log("El personaje está interactuando con la fuente.");
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
 
    public string GetInteractionText()
    {
        return textoInteraccion;
    }
}