using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Controlador principal de Pulse Reflex.
/// Maneja: spawneo de objetos, dificultad progresiva,
/// ensamblado del producto central, timer y estados de juego.
/// </summary>
public class PulseReflexManager : MonoBehaviour
{
    // ─────────────────────────────────────────────
    //  White Label
    // ─────────────────────────────────────────────
    [Header("White Label")]
    [SerializeField] private PulseReflexConfig brandConfig;

    // ─────────────────────────────────────────────
    //  Parámetros de juego
    // ─────────────────────────────────────────────
    [Header("Parámetros de juego")]
    [SerializeField] private float totalTime = 90f;
    [SerializeField] private int hitsToWin = 12;

    [Header("Fase 1 (0 - 20s transcurridos)")]
    [SerializeField] private float phase1VisibleDuration = 2.5f;
    [SerializeField] private int phase1GroupSize = 2;

    [Header("Fase 2 (20s - 50s transcurridos)")]
    [SerializeField] private float phase2VisibleDuration = 2.0f;
    [SerializeField] private int phase2GroupSize = 3;

    [Header("Fase 3 (50s - 90s transcurridos)")]
    [SerializeField] private float phase3VisibleDuration = 1.5f;
    [SerializeField] private int phase3GroupSize = 3;

    [Header("Intervalo entre grupos")]
    [Tooltip("Segundos entre que desaparece un grupo y aparece el siguiente")]
    [SerializeField] private float spawnInterval = 0.4f;

    [Header("Aceleración por aciertos")]
    [Tooltip("Segundos que se restan al tiempo visible por cada acierto")]
    [SerializeField] private float durationDecreasePerHit = 0.15f; 
    
    [Tooltip("El tiempo mínimo absoluto que un objeto puede estar en pantalla")]
    [SerializeField] private float minVisibleDuration = 0.6f; 

    // ─────────────────────────────────────────────
    //  Referencias
    // ─────────────────────────────────────────────
    [Header("Referencias")]
    [SerializeField] private PulseReflexSpawner spawner;
    [SerializeField] private PulseReflexUI ui;

    // ─────────────────────────────────────────────
    //  Eventos
    // ─────────────────────────────────────────────
    [Header("Eventos")]
    public UnityEvent onGameStart;
    public UnityEvent onHitRegistered;
    public UnityEvent onGameWin;
    public UnityEvent onGameLose;

    // ─────────────────────────────────────────────
    //  Estado interno
    // ─────────────────────────────────────────────
    public enum GameState { Idle, Playing, Win, Lose }
    public GameState CurrentState { get; private set; } = GameState.Idle;

    private float remainingTime;
    private float elapsedTime;
    private int hitsLanded;
    private bool gameRunning;

    // Propiedades públicas para UI y spawner
    public float RemainingTime => remainingTime;
    public int HitsLanded => hitsLanded;
    public int HitsToWin => hitsToWin;
    public float HitProgress => Mathf.Clamp01((float)hitsLanded / hitsToWin);
    public PulseReflexConfig BrandConfig => brandConfig;

    // Parámetros de fase actuales (el spawner los consulta)
    public float CurrentVisibleDuration { get; private set; }
    public int CurrentGroupSize { get; private set; }

    // ─────────────────────────────────────────────
    //  Ciclo de vida
    // ─────────────────────────────────────────────
    private void Start()
    {
        Debug.Log("[PulseReflex] Start()");
        Debug.Log($"[PulseReflex] spawner: {spawner != null} | ui: {ui != null} | config: {brandConfig != null}");
        ApplyBrandConfig();
        StartGame();
    }

    private void Update()
    {
        if (!gameRunning) return;
        UpdateTimer();
        UpdateDifficulty();
    }

    // ─────────────────────────────────────────────
    //  Inicialización
    // ─────────────────────────────────────────────
    public void StartGame()
    {
        remainingTime = totalTime;
        elapsedTime = 0f;
        hitsLanded = 0;
        gameRunning = true;
        CurrentState = GameState.Playing;
        
        CurrentVisibleDuration = phase1VisibleDuration;
        CurrentGroupSize = phase1GroupSize;

        spawner?.StartSpawning();
        ui?.UpdateAll(this);
        onGameStart?.Invoke();
        Debug.Log($"[PulseReflex] Juego iniciado. Hits necesarios: {hitsToWin}");
    }

    private void ApplyBrandConfig()
    {
        if (brandConfig == null)
        {
            Debug.LogWarning("[PulseReflex] No hay BrandConfig asignado.");
            return;
        }
        spawner?.ApplyBrand(brandConfig);
        ui?.ApplyBrand(brandConfig);
    }

    // ─────────────────────────────────────────────
    //  Timer
    // ─────────────────────────────────────────────
    private void UpdateTimer()
    {
        remainingTime -= Time.deltaTime;
        elapsedTime += Time.deltaTime;
        ui?.UpdateTimer(remainingTime);

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            ui?.UpdateTimer(0f);
            Debug.Log("[PulseReflex] Tiempo agotado → TriggerLose");
            TriggerLose();
        }
    }

    // ─────────────────────────────────────────────
    //  Dificultad progresiva
    // ─────────────────────────────────────────────
    private void UpdateDifficulty()
    {
        float baseDuration;

        // 1. Determinamos la duración base según la fase de tiempo
        if (elapsedTime < 20f)
        {
            baseDuration = phase1VisibleDuration;
            CurrentGroupSize = phase1GroupSize;
        }
        else if (elapsedTime < 50f)
        {
            baseDuration = phase2VisibleDuration;
            CurrentGroupSize = phase2GroupSize;
        }
        else
        {
            baseDuration = phase3VisibleDuration;
            CurrentGroupSize = phase3GroupSize;
        }

        // 2. Calculamos la nueva duración restando tiempo por cada acierto
        float calculatedDuration = baseDuration - (hitsLanded * durationDecreasePerHit);

        // 3. Nos aseguramos de que no sea imposible de jugar limitándolo al mínimo
        CurrentVisibleDuration = Mathf.Max(calculatedDuration, minVisibleDuration);
    }

    // ─────────────────────────────────────────────
    //  API pública — el spawner llama esto al acertar
    // ─────────────────────────────────────────────
    public void RegisterHit()
    {
        if (!gameRunning) return;

        hitsLanded++;
        ui?.UpdateHitCount(hitsLanded, hitsToWin);
        ui?.UpdatePrizeAssembly(HitProgress);
        onHitRegistered?.Invoke();
        Debug.Log($"[PulseReflex] Hit registrado: {hitsLanded}/{hitsToWin}");

        if (hitsLanded >= hitsToWin)
            TriggerWin();
    }

    // ─────────────────────────────────────────────
    //  Estados finales
    // ─────────────────────────────────────────────
    private void TriggerWin()
    {
        gameRunning = false;
        CurrentState = GameState.Win;
        spawner?.StopSpawning();
        Debug.Log("[PulseReflex] ¡Victoria!");
        StartCoroutine(WinSequence());
    }

    private void TriggerLose()
    {
        gameRunning = false;
        CurrentState = GameState.Lose;
        spawner?.StopSpawning();
        ui?.ShowLoseScreen();
        onGameLose?.Invoke();
        Debug.Log("[PulseReflex] Derrota — LoseScreen activado");
    }

    private IEnumerator WinSequence()
    {
        yield return new WaitForSeconds(1f);
        ui?.ShowWinScreen(brandConfig);
        onGameWin?.Invoke();
    }

    public void RestartGame()
    {
        StopAllCoroutines();
        spawner?.StopSpawning();
        StartGame();
    }

    // Getter para el spawner
    public float SpawnInterval => spawnInterval;
}