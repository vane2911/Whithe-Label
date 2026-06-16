using UnityEngine;
using UnityEngine.Events;
using System.Collections;

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

    [Header("Eventos")]
    public UnityEvent onGameStart;
    public UnityEvent onGameWin;
    public UnityEvent onGameLose;

    public enum GameState { Idle, Playing, Win, Lose }
    public GameState CurrentState { get; private set; } = GameState.Idle;

    private float remainingTime;
    private int punchesLanded;
    private float holdTimer;
    private bool isHolding;
    private bool superPunchReady;

    public float RemainingTime => remainingTime;
    public int PunchesLanded => punchesLanded;
    public int TotalPunches => totalPunches;
    public float HoldProgress => Mathf.Clamp01(holdTimer / holdDuration);

    [Header("Efecto de Destrucción Final")]
    [Tooltip("El costal original que se va a ocultar")]
    [SerializeField] private GameObject costalOriginal;
    
    [Tooltip("El Prefab azul de tu costal roto en pedacitos")]
    [SerializeField] private GameObject prefabCostalRoto;
    
    [Tooltip("El Sprite de la proteína de Smart Fit (con su script de flotar)")]
    [SerializeField] private GameObject recompensaProteina;
    
    [Tooltip("Fuerza con la que salen volando los pedazos")]
    [SerializeField] private float fuerzaExplosion = 500f;
    
    [Tooltip("Radio de la explosión")]
    [SerializeField] private float radioExplosion = 2f;

    [Header("Elementos a Ocultar al Ganar")]
    [Tooltip("La base metálica del costal")]
    [SerializeField] private GameObject baseCostal;
    
    [Tooltip("Las cadenas de donde cuelga")]
    [SerializeField] private GameObject cadenasCostal;

    [Header("Elementos de la UI del Gameplay")]
    [Tooltip("Arrastra aquí el objeto TimerText de la jerarquía")]
    [SerializeField] private GameObject timerTextObject;

    [Tooltip("Arrastra aquí el objeto PunchCountText de la jerarquia")]
    [SerializeField] private GameObject punchCountTextObject; 

    private void Start()
    {
        if (recompensaProteina != null) recompensaProteina.SetActive(false);
        // 1. Aseguramos que el estado inicial sea Idle
        CurrentState = GameState.Idle;

        // 2. Apagamos el Canvas de la interfaz al iniciar
        if (ui != null) ui.gameObject.SetActive(false);

        // 3. Ocultamos los guantes por completo al iniciar
        if (gloveController != null) gloveController.gameObject.SetActive(false);

        // 4. LA MAGIA: Usamos tu propio método para "congelar" el péndulo desde el inicio
        if (punchingBag != null) punchingBag.OnHoldStart();
        SetGameplayUIActive(false);
    }

    private void Update()
    {
        // Si no estamos en el estado Playing, no hacemos nada de lo de abajo
        if (CurrentState != GameState.Playing) return;
        
        // Solo si el juego ya inició (Playing), corre el tiempo y detecta los golpes
        UpdateTimer();
        UpdateInput();
    }

    public void StartGame()
    {
        if (recompensaProteina != null) recompensaProteina.SetActive(false);
        remainingTime = totalTime;
        punchesLanded = 0;
        holdTimer = 0f;
        isHolding = false;
        superPunchReady = false;
        CurrentState = GameState.Playing;

        // 1. Volvemos a encender la interfaz en pantalla
        if (ui != null) ui.gameObject.SetActive(true);

        // 2. Hacemos aparecer los guantes de boxeo
        if (gloveController != null) gloveController.gameObject.SetActive(true);

        // 3. Activamos el script del costal para que empiece a balancearse
        if (punchingBag != null) punchingBag.enabled = true;

        SetGameplayUIActive(true);

        punchingBag?.ResetBag();
        ui?.UpdateAll(this);
        onGameStart?.Invoke();
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
        // 1. Inicia carga
        if (IsInputDown())
        {
            isHolding = true;
            holdTimer = 0f;
            superPunchReady = false;
            punchingBag?.OnHoldStart();
            ui?.ShowHoldBar();
        }

        // 2. Mantiene carga
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

        // 3. Suelta el golpe
        if (isHolding && IsInputUp())
        {
            // BUSCA ESTA LÍNEA O AGREGA LA REFERENCIA A TU GLOVE CONTROLLER
            // Suponiendo que tienes una referencia llamada 'gloveController'
            gloveController?.DetenerCarga(); 

            if (punchingBag != null && punchingBag.IsInCenterZone && holdTimer >= holdDuration)
            {
                RegisterPunch(isSuperPunch: true);
            }
            else
            {
                // Falló por soltar antes de tiempo o no atinarle al centro
                punchingBag?.OnInvalidRelease();
                ui?.ShowInvalidPunchFeedback();
            }

            isHolding = false;
            holdTimer = 0f;
            superPunchReady = false;
            ui?.ResetHoldBar();
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

        // 1. Desaparecemos el costal original
        if (costalOriginal != null) costalOriginal.SetActive(false);
        if (baseCostal != null) baseCostal.SetActive(false);
        if (cadenasCostal != null) cadenasCostal.SetActive(false);
        if (gloveController != null) gloveController.gameObject.SetActive(false);
        SetGameplayUIActive(false);

        // 2. Instanciamos la versión rota y COPIAMOS LA ROTACIÓN
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

        // 3. Recompensa (¡AQUÍ ESTÁ EL CAMBIO!)
        if (recompensaProteina != null)
        {
            // Ya no la teletransportamos, solo la encendemos justo donde tú la dejaste flotando en tu escena
            recompensaProteina.SetActive(true);
        }

        StartCoroutine(WinSequence());
    }

    private void TriggerLose()
    {
        CurrentState = GameState.Lose;
        isHolding = false;
        ui?.ShowLoseScreen();
        onGameLose?.Invoke();
    }

    private void SetGameplayUIActive(bool activeState)
    {
        if (timerTextObject != null) timerTextObject.SetActive(activeState);
        if (punchCountTextObject != null) punchCountTextObject.SetActive(activeState);
    }

    private IEnumerator WinSequence()
    {
        yield return new WaitForSeconds(1.5f);
        ui?.ShowWinScreen(brandConfig);
        onGameWin?.Invoke();
    }

    // ─── LÓGICA DE CONTROLES (Preparado para VR) ───
    
    private bool IsInputDown()
    {
        // Devuelve true si presionas el clic izquierdo (0) O el derecho (1)
        return Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
    }

    private bool IsInputHolding()
    {
        // Devuelve true mientras mantengas presionado cualquiera de los dos
        return Input.GetMouseButton(0) || Input.GetMouseButton(1);
    }

    private bool IsInputUp()
    {
        // Devuelve true en el momento exacto en que sueltas cualquiera de los dos
        return Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1);
    }
}