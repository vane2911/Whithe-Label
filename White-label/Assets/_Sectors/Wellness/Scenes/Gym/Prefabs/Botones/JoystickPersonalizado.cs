using UnityEngine;
using UnityEngine.EventSystems;
using StarterAssets; 

public class JoystickPersonalizado : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Referencias Visuales")]
    public RectTransform fondoJoystick;
    public RectTransform manijaJoystick;

    [Header("Configuración")]
    public float rangoMovimiento = 50f;
    [Tooltip("Píxeles de tolerancia para evitar el latigazo al tocar la pantalla")]
    public float zonaMuerta = 10f; // <-- NUEVA VARIABLE

    [Header("Conexión al Jugador")]
    public StarterAssetsInputs inputsJugador;

    public Vector2 InputVector { get; private set; }
    private Vector2 posicionInicialManija;

    private void Start()
    {
        if (manijaJoystick != null)
        {
            posicionInicialManija = manijaJoystick.anchoredPosition;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 posicionToque;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(fondoJoystick, eventData.position, eventData.pressEventCamera, out posicionToque))
        {
            Vector2 direccion = posicionToque;

            // Limitamos al borde del aro visualmente
            if (direccion.magnitude > rangoMovimiento)
            {
                direccion = direccion.normalized * rangoMovimiento;
            }

            manijaJoystick.anchoredPosition = direccion;

            // --- MAGIA ANTI-LATIGAZOS (ZONA MUERTA) ---
            // Si el toque está dentro del rango de la zona muerta, enviamos cero movimiento
            if (direccion.magnitude < zonaMuerta)
            {
                InputVector = Vector2.zero;
            }
            else
            {
                // Si ya salió de la zona muerta, enviamos la dirección real
                InputVector = direccion / rangoMovimiento;
            }

            if (inputsJugador != null)
            {
                inputsJugador.MoveInput(InputVector);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        InputVector = Vector2.zero;
        manijaJoystick.anchoredPosition = posicionInicialManija;
        
        if (inputsJugador != null)
        {
            inputsJugador.MoveInput(Vector2.zero);
        }
    }
}