using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using StarterAssets; 
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class BrandImpactGameManager : MonoBehaviour
{
    [Header("White Label")]
    [SerializeField] private BrandImpactConfig brandConfig;

    [Header("Parámetros de juego")]
    [SerializeField] private float totalTime = 90f;
    [SerializeField] private float holdDuration = 4f;
    [SerializeField] private int totalPunches = 10;

    [Header("Puntos de Impacto de los Guantes")]
    public Transform puntoImpacto_L;
    public Transform puntoImpacto_R;

    [Header("Referencias")]
    [SerializeField] private PunchingBagController punchingBag;
    [SerializeField] private BrandImpactUI ui;
    [SerializeField] private GloveController gloveController;

    [Header("Sistema de Combate (Fijar Blanco)")]
    [SerializeField] private FijarBlanco sistemaFijado;
    [SerializeField] private Transform costalObjetivo;

    [Header("Conexión Móvil")]
    [Tooltip("Arrastra aquí a tu PlayerArmature")]
    public StarterAssetsInputs inputsJugador;
    [Tooltip("Marca esto para que Unity finja ser un celular mientras editas")]
    public bool simularMovilEnPC = true;

    [Header("Eventos")]
    public UnityEvent onGameStart;
    public UnityEvent onGameWin;
    public UnityEvent onGameLose;

    public enum GameState { Idle, Playing, Win, Lose, Paused }
    public GameState CurrentState { get; private set; } = GameState.Idle;

    private float remainingTime;
    private int punchesLanded;
    private float holdTimer;
    private bool isHolding;
    private bool superPunchReady;
    private bool previousMobileHold = false;

    public float RemainingTime => remainingTime;
    public int PunchesLanded => punchesLanded;
    public int TotalPunches => totalPunches;
    private int totalPunchesAttemped;
    public float HoldProgress => Mathf.Clamp01(holdTimer / holdDuration);

    [Header("Efecto de Destrucción Final")]
    [SerializeField] private GameObject costalOriginal;
    [SerializeField] private GameObject prefabCostalRoto;
    [SerializeField] private GameObject recompensaProteina;
    [SerializeField] private float fuerzaExplosion = 500f;
    [SerializeField] private float radioExplosion = 2f;

    [Header("Elementos a Ocultar al Ganar")]
    [SerializeField] private GameObject baseCostal;
    [SerializeField] private GameObject cadenasCostal;

    [Header("Elementos de la UI del Gameplay")]
    [SerializeField] private GameObject timerTextObject;
    [SerializeField] private GameObject punchCountTextObject; 
    [SerializeField] private GameObject pauseButtonObject;

    //[Header("Física del Costal")]
    //[SerializeField] private CostalPendulo scriptCostalPendulo;

    [Header("Cámara Personalizada")]
    public MultiPerspectiveCamera camaraMultivista;

    private void Start()
    {
        if (recompensaProteina != null) recompensaProteina.SetActive(false);
        CurrentState = GameState.Idle;

       // if (ui != null) ui.gameObject.SetActive(false);
        if (gloveController != null) gloveController.gameObject.SetActive(false);
        if (punchingBag != null) punchingBag.OnHoldStart();

        if (sistemaFijado != null) sistemaFijado.activo = false;
        
        SetGameplayUIActive(false);
    }

    private void Update()
    {
        if (CurrentState == GameState.Paused && Input.GetKeyDown(KeyCode.Escape))
        {
            ReanudarJuego();
            return;
        }

        if(CurrentState != GameState.Playing) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PausarJuego();
            return;
        }
        
        UpdateTimer();
        UpdateInput();
    }

    public void PausarJuego()
    {
        CurrentState = GameState.Paused;
        Time.timeScale = 0f; // 🔴 Congela el universo entero de Unity

        // Liberamos el cursor
        if (inputsJugador != null)
        {
            inputsJugador.cursorSiempreVisible = true; 
            inputsJugador.SetCursorState(false);
            inputsJugador.move = Vector2.zero;   
            inputsJugador.look = Vector2.zero; 
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (camaraMultivista != null) camaraMultivista.lockCamera = true;
        if(pauseButtonObject != null) pauseButtonObject.SetActive(false);

        // Le avisamos a la UI que muestre la pantalla
        ui?.TogglePauseScreen(true);
    }

    public void ReanudarJuego()
    {
        CurrentState = GameState.Playing;
        Time.timeScale = 1f; // 🔴 El universo vuelve a moverse

        // Volvemos a atrapar el cursor
        if (inputsJugador != null)
        {
            inputsJugador.cursorSiempreVisible = false; 
            inputsJugador.SetCursorState(true);
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (camaraMultivista != null) camaraMultivista.lockCamera = false;
        if(pauseButtonObject != null) pauseButtonObject.SetActive(true);


        // Le avisamos a la UI que esconda la pantalla
        ui?.TogglePauseScreen(false);
    }

    public void StartGame()
    {
        Time.timeScale =1f;

        totalPunchesAttemped = 0;

        if (recompensaProteina != null) recompensaProteina.SetActive(false);
        remainingTime = totalTime;
        punchesLanded = 0;
        holdTimer = 0f;
        isHolding = false;
        superPunchReady = false;
        previousMobileHold = false;
        CurrentState = GameState.Playing;

        //if (ui != null) ui.gameObject.SetActive(true);
        if (gloveController != null) gloveController.gameObject.SetActive(true);
        if (punchingBag != null) punchingBag.enabled = true;

        if (sistemaFijado != null)
        {
            sistemaFijado.objetivo = costalObjetivo;
            sistemaFijado.activo = true;
        }

        if (inputsJugador != null)
        {
            inputsJugador.cursorSiempreVisible = false; 
            inputsJugador.tutorialActivo = false;
            inputsJugador.SetCursorState(true);
            inputsJugador.move = Vector2.zero;   
            inputsJugador.look = Vector2.zero; 
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
        }

        // 🔴 ¡LA MAGIA DEL CARRUSEL! Frenamos tu cámara personalizada
        if (camaraMultivista != null)
        {
            camaraMultivista.lockCamera = false;
        }

        // 🟢 3. DESCONGELAR LAS FÍSICAS DEL COSTAL
        /*if (scriptCostalPendulo != null)
        {
            scriptCostalPendulo.estaCongelado = false;
        }*/

        SetGameplayUIActive(true);

        punchingBag?.ResetBag();
        ui?.UpdateAll(this);
        onGameStart?.Invoke();
    }

    public void VolverAlMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    private void UpdateTimer()
    {
        remainingTime -= Time.deltaTime;
        ui?.UpdateTimer(remainingTime);

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            TriggerLose();
        }
    }

    private void UpdateInput()
    {
        if (IsInputDown())
        {
            isHolding = true;
            holdTimer = 0f;
            superPunchReady = false;
            punchingBag?.OnHoldStart();
            ui?.ShowHoldBar();
        }

        if (isHolding && IsInputHolding())
        {
            holdTimer += Time.deltaTime;
            ui?.UpdateHoldBar(HoldProgress);

            if (!superPunchReady && holdTimer >= holdDuration)
            {
                superPunchReady = true;
                ui?.ShowSuperPunchFeedback();
            }
        }

        if (isHolding && IsInputUp())
        {
            totalPunchesAttemped ++;
            
            gloveController?.DetenerCarga(); 

            if (punchingBag != null && punchingBag.IsInCenterZone && holdTimer >= holdDuration)
            {
                RegisterPunch(isSuperPunch: true);
            }
            else
            {
                punchingBag?.OnInvalidRelease();
                ui?.ShowInvalidPunchFeedback();
            }

            isHolding = false;
            holdTimer = 0f;
            superPunchReady = false;
            ui?.ResetHoldBar();
        }

        if (EsDispositivoMovil() && inputsJugador != null)
        {
            previousMobileHold = inputsJugador.isHoldingPunchMobile;
        }
    }

    private void RegisterPunch(bool isSuperPunch)
    {
        punchesLanded++;
        punchingBag?.OnPunchLanded(punchesLanded, totalPunches, isSuperPunch);
        ui?.UpdatePunchCount(punchesLanded, totalPunches);

        if (punchesLanded >= totalPunches)
            TriggerWin();
    }

    private void TriggerWin()
    {
        CurrentState = GameState.Win;
        isHolding = false;

        if (sistemaFijado != null) sistemaFijado.activo = false;

        if (costalOriginal != null) costalOriginal.SetActive(false);
        if (baseCostal != null) baseCostal.SetActive(false);
        if (cadenasCostal != null) cadenasCostal.SetActive(false);
        if (gloveController != null) gloveController.gameObject.SetActive(false);
        SetGameplayUIActive(false);

        if (prefabCostalRoto != null && costalOriginal != null)
        {
            GameObject costalRoto = Instantiate(prefabCostalRoto, costalOriginal.transform.position, costalOriginal.transform.rotation);
            Rigidbody[] pedazos = costalRoto.GetComponentsInChildren<Rigidbody>();
            Vector3 epicentro = costalOriginal.transform.position - (Vector3.up * 0.5f);

            foreach (Rigidbody rb in pedazos)
            {
                rb.AddExplosionForce(fuerzaExplosion, epicentro, radioExplosion);
            }
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

        // 🔴 ¡LA MAGIA DEL CARRUSEL! Frenamos tu cámara personalizada
        if (camaraMultivista != null)
        {
            camaraMultivista.lockCamera = true;
        }

        if (recompensaProteina != null)
        {
            recompensaProteina.SetActive(true);
        }

        StartCoroutine(WinSequence());
    }

    private void TriggerLose()
    {
        CurrentState = GameState.Lose;
        isHolding = false;

        if (sistemaFijado != null) sistemaFijado.activo = false;

        // --- 1. LIMPIEZA VISUAL ---
        if (gloveController != null) gloveController.gameObject.SetActive(false);
        SetGameplayUIActive(false);
        ui?.ResetHoldBar(); 

        // --- 2. CONGELAR COSTAL ---
        // (Si decides descongelarlo, solo quítale las // a la siguiente línea)
        //if (scriptCostalPendulo != null) scriptCostalPendulo.estaCongelado = true;

        // --- 3. CONGELAR JUGADOR Y LIBERAR CURSOR ---
        if (inputsJugador != null)
        {
            inputsJugador.cursorSiempreVisible = true; 
            inputsJugador.SetCursorState(false);
            inputsJugador.move = Vector2.zero;   
            inputsJugador.look = Vector2.zero; 
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // 🔴 ¡LA MAGIA DEL CARRUSEL! Frenamos tu cámara personalizada
        if (camaraMultivista != null)
        {
            camaraMultivista.lockCamera = true;
        }

        // --- 4. MOSTRAR PANTALLA ---
        ui?.ShowLoseScreen(punchesLanded, totalPunchesAttemped);
        onGameLose?.Invoke();
    }

    private void SetGameplayUIActive(bool activeState)
    {
        if (timerTextObject != null) timerTextObject.SetActive(activeState);
        if (punchCountTextObject != null) punchCountTextObject.SetActive(activeState);
        if (pauseButtonObject != null) pauseButtonObject.SetActive(activeState);
    }

    private IEnumerator WinSequence()
    {
        yield return new WaitForSeconds(1.5f);
        ui?.ShowWinScreen(brandConfig, punchesLanded, totalPunchesAttemped);
        onGameWin?.Invoke();
    }

    // ─── LÓGICA DE CONTROLES (Protegida) ───
    
    private bool EsDispositivoMovil()
    {
        bool esMovil = SystemInfo.deviceType == DeviceType.Handheld && !UnityEngine.XR.XRSettings.isDeviceActive;
#if UNITY_EDITOR
        esMovil = simularMovilEnPC;
#endif
        return esMovil;
    }

    private bool IsInputDown()
    {
        if (EsDispositivoMovil() && inputsJugador != null)
            return inputsJugador.isHoldingPunchMobile && !previousMobileHold;
            
        return Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
    }

    private bool IsInputHolding()
    {
        if (EsDispositivoMovil() && inputsJugador != null)
            return inputsJugador.isHoldingPunchMobile;

        return Input.GetMouseButton(0) || Input.GetMouseButton(1);
    }

    private bool IsInputUp()
    {
        if (EsDispositivoMovil() && inputsJugador != null)
            return !inputsJugador.isHoldingPunchMobile && previousMobileHold;

        return Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1);
    }
}