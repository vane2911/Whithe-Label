using UnityEngine;
 
public class BoteBasura : MonoBehaviour, IInteractable
{
    private ItemProgreso itemProgreso;
    private bool yaInteractuado = false;
 
    [Header("WhiteLabel - Texto de Interacción")]
    [Tooltip("Texto que se muestra cuando el jugador apunta al objeto")]
    [SerializeField] private string textoInteraccion;
 
    [Tooltip("Texto que se muestra cuando el objeto ya fue interactuado")]
    [SerializeField] private string textoYaInteractuado;
 
    void Start()
    {
        itemProgreso = GetComponent<ItemProgreso>();
    }
 
    public void Interact()
    {
        if (!yaInteractuado && itemProgreso != null)
        {
            itemProgreso.RegistrarProgreso();
            yaInteractuado = true;
        }
    }
 
    public void OnFocus()
    {
        Debug.Log("Viendo el bote de basura");
    }
 
    public void OnLoseFocus()
    {
        // Aquí puedes quitar efecto visual si lo agregas
    }
 
    public string GetInteractionText()
    {
        if (SaveManager.Instance != null &&
            SaveManager.Instance.misDatos.objetosInteractuados.Contains(itemProgreso.idUnico))
        {
            return textoYaInteractuado;
        }
        return textoInteraccion;
    }
}