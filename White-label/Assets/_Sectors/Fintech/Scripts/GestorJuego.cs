using UnityEngine;
using TMPro; 
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class GestorJuego : MonoBehaviour
{
    [Header("Estadísticas de la Partida")]
    public int vidas = 5;
    public int puntos = 0;
    
    public bool juegoTerminado = false; 

    [Header("Interfaz (UI)")]
    public GameObject panelInicio; // ¡NUEVO! Tu pantalla principal
    public GameObject panelHUD; 
    public TextMeshProUGUI textoPuntos; 
    public Image[] imagenCorazones; 

    [Header("Pantallas Finales")] 
    public GameObject panelVictoria; 
    public GameObject panelDerrota;  

    void Start()
    {
        juegoTerminado = false;
        vidas = 5; 
        puntos = 0;
        
        // 1. Empezamos el juego CONGELADO para esperar al jugador
        Time.timeScale = 0f; 

        // 2. Encendemos el Menú de Inicio y apagamos todo lo demás
        if (panelInicio != null) panelInicio.SetActive(true);
        if (panelHUD != null) panelHUD.SetActive(false);
        if (panelVictoria != null) panelVictoria.SetActive(false);
        if (panelDerrota != null) panelDerrota.SetActive(false);

        for (int i = 0; i < imagenCorazones.Length; i++)
        {
            if (imagenCorazones[i] != null)
            {
                imagenCorazones[i].enabled = true;
            }
        }

        ActualizarPantalla(); 

        // 3. Liberamos el mouse para que puedan darle a "Jugar"
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // --- ¡NUEVA FUNCIÓN PARA EL BOTÓN DE INICIO! ---
    public void IniciarVuelo()
    {
        // Apagamos el menú, encendemos el HUD y descongelamos el tiempo
        if (panelInicio != null) panelInicio.SetActive(false);
        if (panelHUD != null) panelHUD.SetActive(true);
        
        Time.timeScale = 1f; 

        // Ocultamos el mouse para que no estorbe al jugar
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void SumarPuntos(int cantidad)
    {
        if (juegoTerminado) return; 

        puntos += cantidad;
        ActualizarPantalla(); 
    }

    void ActualizarPantalla()
    {
        if (textoPuntos != null)
        {
            textoPuntos.text = "Puntos: " + puntos.ToString();
        }
    }

    public void PerderVida()
    {
        if (juegoTerminado) return; 

        vidas -= 1;
        
        if (vidas >= 0 && vidas < imagenCorazones.Length)
        {
            imagenCorazones[vidas].enabled = false; 
        }
        
        if (vidas <= 0)
        {
            PerderJuego();
        }
    }

    public void GanarJuego()
    {
        if (juegoTerminado) return; 

        juegoTerminado = true;
        Debug.Log("¡Viaje completado! Has alcanzado la velocidad máxima.");
        
        // Apagamos el HUD y encendemos el panel de victoria
        if (panelHUD != null) panelHUD.SetActive(false);
        if (panelVictoria != null) panelVictoria.SetActive(true);
        Time.timeScale = 0f; 

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void PerderJuego()
    {
        if (juegoTerminado) return;

        juegoTerminado = true;
        Debug.Log("Vuelo interrumpido. Te quedaste sin vidas.");
        
        // Apagamos el HUD y encendemos el panel de derrota
        if (panelHUD != null) panelHUD.SetActive(false);
        if (panelDerrota != null) panelDerrota.SetActive(true);
        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ReiniciarJuego()
    {
        // Recarga la escena, volviendo automáticamente al Start() y al Menú de Inicio
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}