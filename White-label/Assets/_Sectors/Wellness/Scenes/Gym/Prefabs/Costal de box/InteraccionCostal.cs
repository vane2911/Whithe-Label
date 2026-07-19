using UnityEngine;
using UnityEngine.XR; // Necesario para detectar si hay VR

public class InteraccionCostal : MonoBehaviour, IInteractable
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí el objeto que tiene tu BrandImpactGameManager")]
    [SerializeField] private BrandImpactGameManager gameManager;

    [Header("Textos de Interacción")]
    [SerializeField] private string textoInteraccion = "Presiona [E] para entrenar";

    [Header("Efecto Visual (Halo Glint)")]
    [Tooltip("Arrastra aquí el objeto de tu Particle System (el destello)")]
    [SerializeField] private GameObject luzDestello;

    [Header("Botones Táctiles Móviles")]
    [Tooltip("Arrastra aquí tu botón de interacción (la letra E)")]
    [SerializeField] private GameObject botonMovil_E;
    [Tooltip("Arrastra aquí tu botón del guante Izquierdo (L)")]
    [SerializeField] private GameObject botonMovil_L;
    [Tooltip("Arrastra aquí tu botón del guante Derecho (R)")]
    [SerializeField] private GameObject botonMovil_R;

    [Header("Pruebas en PC")]
    [Tooltip("Marca esto para que el costal muestre los botones táctiles en la computadora")]
    [SerializeField] private bool simularMovilEnPC = true;

    private void Start()
    {
        // Al iniciar el juego, nos aseguramos de que todos estos botones estén apagados
        if (botonMovil_E != null) botonMovil_E.SetActive(false);
        if (botonMovil_L != null) botonMovil_L.SetActive(false);
        if (botonMovil_R != null) botonMovil_R.SetActive(false);
    }

    // Función auxiliar para saber si debemos mostrar botones de pantalla
    private bool EsDispositivoMovil()
    {
        bool esMovil = SystemInfo.deviceType == DeviceType.Handheld && !XRSettings.isDeviceActive;

        // --- MAGIA PARA PRUEBAS EN PC ---
#if UNITY_EDITOR
        esMovil = simularMovilEnPC;
#endif

        return esMovil;
    }

    public void Interact()
    {
        // Si el juego está en estado Idle, lo iniciamos
        if (gameManager != null && gameManager.CurrentState == BrandImpactGameManager.GameState.Idle)
        {
            gameManager.StartGame();
            
            // Apagamos la luz definitivamente cuando el minijuego comience
            if (luzDestello != null) luzDestello.SetActive(false);

            // --- LÓGICA MÓVIL: Cambio de botones ---
            // 1. Ocultamos la E porque ya interactuó
            if (botonMovil_E != null) botonMovil_E.SetActive(false);

            // 2. Si es celular, prendemos los guantes para empezar a pelear
            if (EsDispositivoMovil())
            {
                if (botonMovil_L != null) botonMovil_L.SetActive(true);
                if (botonMovil_R != null) botonMovil_R.SetActive(true);
            }
        }
    }

    public void OnFocus()
    {
        // Cuando el rayo del jugador toca el costal, apagamos el destello
        if (luzDestello != null) luzDestello.SetActive(false);

        // --- LÓGICA MÓVIL: Mostrar la 'E' ---
        // Si es un celular y el juego aún no empieza, prendemos el botón de la 'E'
        if (EsDispositivoMovil() && botonMovil_E != null && gameManager != null && gameManager.CurrentState == BrandImpactGameManager.GameState.Idle)
        {
            botonMovil_E.SetActive(true);
        }
    }

    public void OnLoseFocus()
    {
        // Si dejas de mirar el costal Y el juego aún no empieza, volvemos a prender la luz
        if (luzDestello != null && gameManager != null && gameManager.CurrentState == BrandImpactGameManager.GameState.Idle)
        {
            luzDestello.SetActive(true);
        }

        // --- LÓGICA MÓVIL: Ocultar la 'E' ---
        // Si el jugador se aleja, escondemos el botón
        if (botonMovil_E != null)
        {
            botonMovil_E.SetActive(false);
        }
    }

    public string GetInteractionText()
    {
        // Si el juego ya empezó, ocultamos el texto para que no estorbe en la pantalla
        if (gameManager != null && gameManager.CurrentState != BrandImpactGameManager.GameState.Idle)
        {
            return ""; 
        }
        
        // Si el juego no ha empezado, mostramos el mensaje
        return textoInteraccion;
    }
}