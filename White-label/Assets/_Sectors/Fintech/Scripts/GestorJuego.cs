using UnityEngine;
using TMPro; 
using UnityEngine.UI;
using StarterAssets;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using Cinemachine;

public class GestorJuego : MonoBehaviour
{
    [Header("Controles")]
    public StarterAssetsInputs inputsJugador;
    public MultiPerspectiveCamera camaraMultivista;

    [Header("Estadísticas de la Partida")]
    public int vidas = 3;
    public int puntos = 0;    
    public int aros = 0;
    private float tiempoJugado = 0f;

    public bool juegoTerminado = false; 
    private bool juegoPausado = false;

    public static bool reiniciarDirecto = false;

    [Header("Cámaras")] 
    public GameObject camaraMenu;  
    public GameObject camaraJuego; 

    [Header("Interfaz (UI)")]
    public GameObject panelInicio; 
    public GameObject panelInstrucciones; 
    public ScrollRect scrollInstrucciones;
    public GameObject panelHUD; 
    public GameObject panelPausa;
    public TextMeshProUGUI textoPuntos; 
    public TextMeshProUGUI textoPuntosFinalesG;
    public TextMeshProUGUI textoArosCruzadosG;
    public TextMeshProUGUI textoTiempoG;
    public TextMeshProUGUI textoPuntosFinalesP;
    public TextMeshProUGUI textoArosCruzadosP;
    public TextMeshProUGUI textoTiempoP;
    public Image[] imagenCorazones; 

    [Header("Pantallas Finales")] 
    public GameObject panelVictoria; 
    public GameObject panelDerrota;  

    [Header("Audio")]
    public AudioSource fuenteEfectos;
    public AudioSource fuenteMusica;

    [Header("Clips de audio - efectos")]
    public AudioClip sonidoMoneda;
    public AudioClip sonidoAroExito;
    public AudioClip sonidoAroFallo;
    public AudioClip sonidoSalto;
    public AudioClip sonidoVictoria;
    public AudioClip sonidoDerrota;

    [Header("Clips de audio - musica")]
    public AudioClip musicaMenu;
    public AudioClip musicaFondo;

    [Header("Sistema de Estrellas (Victoria)")]
    [Tooltip("Arrastra aquí las 3 imágenes amarillas de RELLENO (los hijos 'Image') en orden (Izquierda, Centro, Derecha)")]
    public GameObject[] rellenosEstrellas;

    void Start()
    {
        juegoTerminado = false;
        puntos = 0;
        
        Time.timeScale = 0f; 

        if (rellenosEstrellas != null)
        {
            foreach(GameObject estrella in rellenosEstrellas)
            {
                if (estrella != null) estrella.SetActive(false);
            }
        }

        if (camaraMenu != null) camaraMenu.SetActive(true);
        if (camaraJuego != null) camaraJuego.SetActive(false);

        if (panelInicio != null) panelInicio.SetActive(true);
        if (panelInstrucciones != null) panelInstrucciones.SetActive(false); 
        if (panelPausa != null) panelPausa.SetActive(false);
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

        if (reiniciarDirecto)
        {
            reiniciarDirecto = false; 
            IniciarVuelo(); 
        }
        else
        {
            if (inputsJugador != null)
            {
                inputsJugador.cursorSiempreVisible = true; 
                inputsJugador.SetCursorState(false);
                inputsJugador.move = Vector2.zero;   
                inputsJugador.look = Vector2.zero; 
                
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            ReproducirMusica(musicaMenu);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool viendoInstrucciones = (panelInstrucciones != null && panelInstrucciones.activeSelf);
            bool enMenuInicio = (panelInicio != null && panelInicio.activeSelf);

            if (viendoInstrucciones)
            {
                CerrarInstrucciones();
            }
            else if (!enMenuInicio && !juegoTerminado)
            {
                if (juegoPausado)
                {
                    ReanudarJuego();
                }
                else
                {
                    PausarJuego();
                }
            }
        }

        if (!juegoPausado && !juegoTerminado && panelHUD != null && panelHUD.activeSelf)
        {
            tiempoJugado += Time.deltaTime;
        }
    }

    public void PausarJuego()
    {
        juegoPausado = true;
        Time.timeScale = 0f;

        if (panelPausa != null) panelPausa.SetActive(true);
        if (panelHUD != null) panelHUD.SetActive(false); 

        if (inputsJugador != null)
        {
            inputsJugador.cursorSiempreVisible = true; 
            inputsJugador.SetCursorState(false);
            inputsJugador.move = Vector2.zero;   
            inputsJugador.look = Vector2.zero; 
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (camaraMultivista != null)
        {
            camaraMultivista.lockCamera = true;
        }    
    }

    public void ReanudarJuego()
    {
        juegoPausado = false;
        Time.timeScale = 1f;

        if (panelPausa != null) panelPausa.SetActive(false);
        if (panelInstrucciones != null) panelInstrucciones.SetActive(false); 
        if (panelHUD != null) panelHUD.SetActive(true); 

        if (inputsJugador != null)
        {
            inputsJugador.cursorSiempreVisible = false; 
            inputsJugador.SetCursorState(true);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (camaraMultivista != null) camaraMultivista.lockCamera = false;
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        reiniciarDirecto = false; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void IniciarVuelo()
    {
        if (camaraMenu != null) camaraMenu.SetActive(false);
        if (camaraJuego != null) camaraJuego.SetActive(true);

        if (panelInicio != null) panelInicio.SetActive(false);
        if (panelHUD != null) panelHUD.SetActive(true);
        
        Time.timeScale = 1f; 

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        ReproducirMusica(musicaFondo);
    }

    public void SumarPuntos(int cantidad)
    {
        if (juegoTerminado) return; 

        puntos += cantidad;
        ActualizarPantalla(); 
    }

    public void SumarAros(int cantidad)
    {
        if (juegoTerminado) return;
        aros += cantidad;
        ActualizarPantalla();
    }

    void ActualizarPantalla()
    {
        if (textoPuntos != null)
        {
            textoPuntos.text = puntos.ToString();
        }
        
    }

    public void ReproducirSonidoMoneda()
    {
        if (fuenteEfectos != null && sonidoMoneda != null)
        {
            fuenteEfectos.PlayOneShot(sonidoMoneda);
        }
    }

    public void ReproducirSonidoExito()
    {
        if (fuenteEfectos != null && sonidoAroExito != null)
        {
            fuenteEfectos.PlayOneShot(sonidoAroExito);
        }
    }

    public void ReproducirSonidoSalto()
    {
        if (fuenteEfectos != null && sonidoSalto != null) fuenteEfectos.PlayOneShot(sonidoSalto);
    }

    public void ReproducirSonidoVictoria()
    {
        if (fuenteEfectos != null && sonidoVictoria != null) fuenteEfectos.PlayOneShot(sonidoVictoria);
    }

    public void ReproducirSonidoDerrota()
    {
        if (fuenteEfectos != null && sonidoDerrota != null) fuenteEfectos.PlayOneShot(sonidoDerrota);
    }

    public void ReproducirMusica(AudioClip cancion)
    {
        if (fuenteMusica != null && cancion != null)
        {
            fuenteMusica.Stop();
            fuenteMusica.clip = cancion;
            fuenteMusica.loop = true;
            fuenteMusica.Play();
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

        if (fuenteMusica != null) fuenteMusica.Stop();
        if (fuenteEfectos != null && sonidoVictoria != null) fuenteEfectos.PlayOneShot(sonidoVictoria);
        
        if (panelHUD != null) panelHUD.SetActive(false);
        if (panelVictoria != null) panelVictoria.SetActive(true);

        if (textoPuntosFinalesG != null) textoPuntosFinalesG.text = puntos.ToString();
        if(textoArosCruzadosG != null) textoArosCruzadosG.text = aros.ToString();

        if (textoTiempoG != null)
        {
            int minutos = Mathf.FloorToInt(tiempoJugado / 60f);
            int segundos = Mathf.FloorToInt(tiempoJugado - minutos * 60);
            textoTiempoG.text = string.Format("{0}:{1:00}", minutos, segundos);
        }

        if (rellenosEstrellas != null && rellenosEstrellas.Length == 3)
        {
            foreach(GameObject estrella in rellenosEstrellas)
            {
                if (estrella != null) estrella.SetActive(false);
            }

        }
            for (int i = 0; i < vidas; i++)
        {
            if (i < rellenosEstrellas.Length && rellenosEstrellas[i] != null)
            {
                rellenosEstrellas[i].SetActive(true);
            }
        }

        Time.timeScale = 0f;

        if (inputsJugador != null)
        {
            inputsJugador.cursorSiempreVisible = true; 
            inputsJugador.SetCursorState(false);
            inputsJugador.move = Vector2.zero;   
            inputsJugador.look = Vector2.zero; 
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (camaraMultivista != null)
        {
            camaraMultivista.lockCamera = true;
        }    
    }

    public void PerderJuego()
    {
        if (juegoTerminado) return;

        juegoTerminado = true;

        if (fuenteMusica != null) fuenteMusica.Stop();

        if (fuenteEfectos != null && sonidoDerrota != null) fuenteEfectos.PlayOneShot(sonidoDerrota);
        
        if (panelHUD != null) panelHUD.SetActive(false);
        if (panelDerrota != null) panelDerrota.SetActive(true);
        Time.timeScale = 0f;

        if (textoPuntosFinalesP != null) textoPuntosFinalesP.text = puntos.ToString();
        if(textoArosCruzadosP != null) textoArosCruzadosP.text = aros.ToString();

        if (textoTiempoP != null)
        {
            int minutos = Mathf.FloorToInt(tiempoJugado / 60f);
            int segundos = Mathf.FloorToInt(tiempoJugado - minutos * 60);
            textoTiempoP.text = string.Format("{0}:{1:00}", minutos, segundos);
        }

        if (inputsJugador != null)
        {
            inputsJugador.cursorSiempreVisible = true; 
            inputsJugador.SetCursorState(false);
            inputsJugador.move = Vector2.zero;   
            inputsJugador.look = Vector2.zero; 
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (camaraMultivista != null)
        {
            camaraMultivista.lockCamera = true;
        }    
    }

    public void ReiniciarJuego()
    {
        reiniciarDirecto = true; 
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MostrarInstrucciones()
    {
        if (inputsJugador != null)
        {
            inputsJugador.cursorSiempreVisible = true; 
            inputsJugador.SetCursorState(false);
            inputsJugador.move = Vector2.zero;   
            inputsJugador.look = Vector2.zero; 
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (panelInstrucciones != null) panelInstrucciones.SetActive(true);

        if (juegoPausado)
        {
            if (panelPausa != null) panelPausa.SetActive(false);
        }
        else
        {
            if (panelInicio != null) panelInicio.SetActive(false);
        }

        if (scrollInstrucciones != null)
        {
            scrollInstrucciones.verticalNormalizedPosition = 1f;
        }
    }

    public void CerrarInstrucciones()
    {
        if (panelInstrucciones != null) panelInstrucciones.SetActive(false);

        if (juegoPausado)
        {
            if (panelPausa != null) panelPausa.SetActive(true);
        }
        else
        {
            if (panelInicio != null) panelInicio.SetActive(true);
        }
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego");
        Application.Quit();
    }
}