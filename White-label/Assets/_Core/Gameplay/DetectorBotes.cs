using UnityEngine;

// Al agregar IInteractable, el PlayerInteraction lo reconocerá automáticamente
public class BoteBasura : MonoBehaviour, IInteractable
{
    private ItemProgreso itemProgreso;
    private bool yaInteractuado = false;

    void Start()
    {
        // Buscamos el componente de progreso que ya tienes
        itemProgreso = GetComponent<ItemProgreso>();
    }

    public void Interact()
    {
        if (!yaInteractuado && itemProgreso != null)
        {
            itemProgreso.RegistrarProgreso();
            yaInteractuado = true; // Evita que sume puntos infinitos
        }
    }

    public void OnFocus()
    {
        // Aquí podrías poner que el bote brille igual que la fuente si quieres
        Debug.Log("Viendo el bote de basura");
    }

    public void OnLoseFocus()
    {
        // Aquí quitas el brillo
    }

    // Agrégalo al final de tu clase BoteBasura
    public string GetInteractionText() {
        // Consultamos al SaveManager si este ID ya fue recolectado
        if (SaveManager.Instance != null && SaveManager.Instance.misDatos.objetosInteractuados.Contains(itemProgreso.idUnico)) {
            return "Ya recolectado";
        }
        return "Presiona [E] para recoger basura";
    }
}