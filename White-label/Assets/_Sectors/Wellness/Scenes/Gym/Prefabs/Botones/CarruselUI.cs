using UnityEngine;
using StarterAssets;

public class CarruselUI : MonoBehaviour
{
    [Header("Configuración del Arco")]
    public float radio = 300f; 
    public float separacionAngular = 35f; 
    public float velocidadGiro = 5f;

    [Header("Conexiones maestras")]
    public StarterAssetsInputs inputsJugador; 
    public MultiPerspectiveCamera camaraMultivista; // ¡Tu script real de cámara!
    public ThirdPersonController controladorJugador;

    [Header("UI a ocultar")]
    public GameObject contenedorBotones;
    
    [Header("Objetos de Escena")]
    public GameObject costalEscena;

    private Transform[] paneles;
    private int indiceActual = 0;
    private float anguloObjetivo = 0f;

    void Start()
    {
        if (costalEscena != null) costalEscena.SetActive(false);
        
        paneles = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            paneles[i] = transform.GetChild(i);
        }

        AcomodarEnSemicirculo();
        
        // --- ENCENDEMOS EL TUTORIAL ---
        if (inputsJugador != null)
        {
            inputsJugador.cursorSiempreVisible = true; // ¡Evita que el jugador robe el mouse!
            inputsJugador.SetCursorState(false);
            inputsJugador.tutorialActivo = true; // Frena al personaje
            inputsJugador.move = Vector2.zero;   
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // ¡Frenamos tu cámara personalizada!
        if (camaraMultivista != null)
        {
            camaraMultivista.lockCamera = true;
        }
    }

    void Update()
    {
        Quaternion rotacionDeseada = Quaternion.Euler(0, anguloObjetivo, 0);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, rotacionDeseada, Time.deltaTime * velocidadGiro);
    }


    void AcomodarEnSemicirculo()
    {
        for (int i = 0; i < paneles.Length; i++)
        {
            float anguloActual = i * separacionAngular;
            
            // Calculamos X y Z puras, sin restarle el radio al final
            float posX = Mathf.Sin(anguloActual * Mathf.Deg2Rad) * radio;
            float posZ = Mathf.Cos(anguloActual * Mathf.Deg2Rad) * radio;
            
            paneles[i].localPosition = new Vector3(posX, 0, posZ);
            paneles[i].localRotation = Quaternion.Euler(0, anguloActual, 0);
        }
    }

    public void CambiarPanel(int direccion)
    {
        Debug.Log("Clic recibido en el panel: " + indiceActual);
        if (direccion > 0 && indiceActual == paneles.Length - 1) 
        {
            CerrarTutorial();
            return;
        }

        // Sumamos o restamos dependiendo del botón (1 o -1)
        indiceActual += direccion;
        
        // El Clamp evita que bajemos de 0 si el jugador spamea el botón "Atrás"
        indiceActual = Mathf.Clamp(indiceActual, 0, paneles.Length - 1);
        anguloObjetivo = -indiceActual * separacionAngular;
    }

   public void CerrarTutorial()
    {
        Debug.Log("Cerrando tutorial...");
        gameObject.SetActive(false); 

        if (costalEscena != null) costalEscena.SetActive(true);
        
        // Control directo que ya te funciona
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (inputsJugador != null)
        {
            inputsJugador.tutorialActivo = false;
        }
        
        if (camaraMultivista != null)
        {
            camaraMultivista.lockCamera = false;
        }

        if (contenedorBotones != null)
        {
            contenedorBotones.SetActive(false);
        }

    }
}