using UnityEngine;
using UnityEngine.UI; // Súper importante para poder usar "Image"
using TMPro;
using UnityEngine.XR;

public class PlataformaTexto : MonoBehaviour
{
    [Header("Referencias de UI")]
    public TextMeshProUGUI componenteTexto;
    public Image componenteImagen; // ¡NUEVO! Aquí arrastras la imagen del panel

    [Header("Contenido por Plataforma (Textos)")]
    [TextArea(3, 10)] public string textoPC;
    [TextArea(3, 10)] public string textoMovil;
    [TextArea(3, 10)] public string textoVR;

    [Header("Contenido por Plataforma (Íconos)")]
    public Sprite iconoPC;    // Ícono de ratón/teclado
    public Sprite iconoMovil; // Ícono de un dedo deslizando/celular
    public Sprite iconoVR;    // Ícono de controles VR

    [Header("Configuración de Prueba")]
    public bool simularMovilEnPC = true;

    // Se ejecuta al iniciar el juego
    void Start()
    {
        ActualizarUI();
    }

    public void ActualizarUI()
    {
        // 1. Detectar VR
        if (XRSettings.isDeviceActive)
        {
            AplicarContenido(textoVR, iconoVR);
            return;
        }

        // 2. Detectar Móvil
        bool esMovil = SystemInfo.deviceType == DeviceType.Handheld;
        
        #if UNITY_EDITOR
        esMovil = simularMovilEnPC;
        #endif

        // 3. Asignar contenido según plataforma
        if (esMovil)
        {
            AplicarContenido(textoMovil, iconoMovil);
        }
        else
        {
            AplicarContenido(textoPC, iconoPC);
        }
    }

    // Método auxiliar para mantener todo limpio y no repetir código
    private void AplicarContenido(string texto, Sprite icono)
    {
        if (componenteTexto != null) 
        {
            componenteTexto.text = texto;
        }

        if (componenteImagen != null)
        {
            if (icono != null)
            {
                componenteImagen.sprite = icono;
                componenteImagen.gameObject.SetActive(true); // La mostramos
            }
            else
            {
                componenteImagen.gameObject.SetActive(false); // La ocultamos si no hay ícono
            }
        }
    }
}