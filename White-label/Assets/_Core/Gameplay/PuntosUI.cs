using UnityEngine;
using TMPro;

public class PuntosUI : MonoBehaviour
{
    [Header("Referencias UI")]
    [Tooltip("El componente de texto donde se mostrarán los puntos")]
    public TextMeshProUGUI textoUI;
 
    [Header("WhiteLabel - Personalización")]
    [Tooltip("Etiqueta que aparece antes del número. Ej: 'Puntos', 'Tokens', 'Logros'")]
    [SerializeField] private string prefijo = "Puntos";
 
    private void OnEnable()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.OnDatosActualizados += ActualizarTexto;
            ActualizarTexto(SaveManager.Instance.misDatos);
        }
    }
 
    private void OnDisable()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.OnDatosActualizados -= ActualizarTexto;
        }
    }
 
    private void ActualizarTexto(DatosJuego datos)
    {
        if (textoUI != null)
        {
            textoUI.text = $"{prefijo}: {datos.puntos}";
        }
    }
}