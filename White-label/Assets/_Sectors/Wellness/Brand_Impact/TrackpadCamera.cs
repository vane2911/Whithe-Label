using UnityEngine;
using UnityEngine.EventSystems;
using StarterAssets;

public class TrackpadCamara : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Conexión")]
    [Tooltip("Arrastra aquí a tu PlayerArmature")]
    public StarterAssetsInputs inputsJugador;

    [Header("Configuración")]
    [Tooltip("Ajusta este número si la cámara gira muy lento o muy rápido")]
    public float sensibilidadCamara = 0.5f;

    public void OnPointerDown(PointerEventData eventData)
    {
        // El dedo acaba de tocar la pantalla (no necesitamos hacer nada especial aquí)
    }

    public void OnDrag(PointerEventData eventData)
    {
        // El dedo se está deslizando
        if (inputsJugador != null)
        {
            // eventData.delta nos da exactamente cuántos píxeles se movió el dedo
            Vector2 movimientoDedo = eventData.delta * sensibilidadCamara;
            
            // Le mandamos este movimiento al mensajero para que gire la cámara
            inputsJugador.LookInput(movimientoDedo);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Al levantar el dedo, le decimos a la cámara que deje de girar
        if (inputsJugador != null)
        {
            inputsJugador.LookInput(Vector2.zero);
        }
    }
}