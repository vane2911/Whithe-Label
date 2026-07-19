using UnityEngine;
using UnityEngine.UI; // Necesario para cambiar la imagen del botón

public class ControlSonido : MonoBehaviour
{
    private bool sonidoActivado = true;

    [Header("Iconos del Botón")]
    public Image iconoBoton; 
    public Sprite iconoSonidoOn; 
    public Sprite iconoSonidoOff; 

    void Start()
    {
        AudioListener.volume = 1f;
        sonidoActivado = true;
        ActualizarIcono();
    }

    public void AlternarSonido()
    {
        sonidoActivado = !sonidoActivado; 

        if (sonidoActivado)
        {
            AudioListener.volume = 1f; 
        }
        else
        {
            AudioListener.volume = 0f; 
        }

        ActualizarIcono();
    }

    private void ActualizarIcono()
    {
        if (iconoBoton != null && iconoSonidoOn != null && iconoSonidoOff != null)
        {
            iconoBoton.sprite = sonidoActivado ? iconoSonidoOn : iconoSonidoOff;
        }
    }
}