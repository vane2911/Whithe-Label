using UnityEngine;

public class Interaccion : MonoBehaviour
{
    [Header("Interfaz de Usuario")]
    // Aquí arrastrarás tus objetos del Canvas
    public GameObject text1; // Arrastra tu "Text1" aquí
    public GameObject panel; // Arrastra tu "Panel" aquí

    [Header("Configuración del Objeto")]
    public Color colorAlInteractuar = Color.green; // Elige el color desde el Inspector

    private bool estaCerca = false;
    private bool interactuado = false;
    private Renderer rend;

    void Start()
    {
        // Obtenemos el componente que pinta el cubo para poder cambiarle el color
        rend = GetComponent<Renderer>();

        // Por seguridad, nos aseguramos de que los textos estén apagados al iniciar el juego
        if (text1 != null) text1.SetActive(false);
        if (panel != null) panel.SetActive(false);
    }

    void Update()
    {
        // Si el jugador está en el área, no ha interactuado aún y presiona la tecla E
        if (estaCerca && !interactuado && Input.GetKeyDown(KeyCode.E))
        {
            Interactuar();
        }
    }

    void Interactuar()
    {
        interactuado = true;
        
        // 1. Cambiamos el color
        if (rend != null)
        {
            rend.material.color = colorAlInteractuar;
        }

        // 2. Ocultamos el mensaje de "Presiona E" (Text1)
        text1.SetActive(false);

        // 3. Mostramos la ventana de "Lo lograste" (Panel)
        panel.SetActive(true);

        // EXTRA: Si quieres que el panel desaparezca solo después de 3 segundos, 
        // quita las dos barras "//" de la línea de abajo:
        // Invoke("OcultarPanel", 3f);
    }

    // Esta función la usa el Invoke de arriba
    void OcultarPanel()
    {
        panel.SetActive(false);
    }

    // Detecta cuando el PlayerArmature entra al "aura" (el BoxCollider con Is Trigger)
    private void OnTriggerEnter(Collider other)
    {
        // Asegúrate de que tu PlayerArmature tenga el Tag "Player" arriba en el Inspector
        if (other.CompareTag("Player") && !interactuado)
        {
            estaCerca = true;
            text1.SetActive(true); // Muestra el mensaje de ayuda
        }
    }

    // Detecta cuando el PlayerArmature se aleja
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            estaCerca = false;
            text1.SetActive(false); // Oculta el mensaje si te vas sin interactuar
        }
    }
}