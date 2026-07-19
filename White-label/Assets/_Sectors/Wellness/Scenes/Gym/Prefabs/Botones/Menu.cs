using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [Header("Cámaras y Jugador")]
    public GameObject camaraMenu;
    public GameObject camaraPrincipal;
    public GameObject jugador; 

    [Header("Interfaces")]
    public GameObject canvasInicio;
    public GameObject motorTutorial;

    [Header("Transición")]
    public Image pantallaNegra; 

    // Variable espía para saber si ya habíamos apagado el menú
    private bool menuEstabaApagado = false;

    void Start()
    {
        Debug.LogWarning("START DEL MENÚ: El script acaba de arrancar. ¿Se reinició la escena?");

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (jugador != null) jugador.SetActive(false);
        camaraMenu.SetActive(true);
        canvasInicio.SetActive(true);
        camaraPrincipal.SetActive(false);
        motorTutorial.SetActive(false);

        if (pantallaNegra != null) pantallaNegra.gameObject.SetActive(false);
        
        menuEstabaApagado = false;
    }

    void OnEnable()
    {
        // Ignoramos los primeros 2 segundos para que no grite cuando arranca el juego
        if (Time.time > 2f) 
        {
            Debug.LogError("🚨 ¡TE ATRAPÉ FANTASMA! El script Menu está escondido dentro del objeto llamado: [" + gameObject.name + "]. ¡Búscalo en tu jerarquía!");
        }
    }

    public void IniciarEntrenamiento()
    {
        if (pantallaNegra != null) pantallaNegra.gameObject.SetActive(true);

        canvasInicio.SetActive(false);
        camaraMenu.SetActive(false);

        if (jugador != null) jugador.SetActive(true);
        camaraPrincipal.SetActive(true);
        motorTutorial.SetActive(true);

        menuEstabaApagado = true;
        Debug.Log("MENÚ APAGADO: El jugador inició el entrenamiento.");
    }

    void Update()
    {
        if (menuEstabaApagado && canvasInicio != null && canvasInicio.activeInHierarchy)
        {
            //Debug.LogError("Alguien encendió el Canvas Inicio a mis espaldas.");
            
            // Lo regresamos a false para que no te sature la consola con el mismo error
            menuEstabaApagado = false; 
        }
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo de la Aplicacion");
        Application.Quit();
    }
}