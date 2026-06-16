using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Maneja el spawneo, posicionamiento y ciclo de vida
/// de los objetos tocables en Pulse Reflex.
/// v2: excluye zona central donde está el premio,
///     y la dificultad progresiva aumenta velocidad correctamente.
/// </summary>
public class PulseReflexSpawner : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PulseReflexManager gameManager;
    [SerializeField] private RectTransform spawnArea;
    [SerializeField] private GameObject objectPrefab;

    [Header("Pool de objetos")]
    [SerializeField] private int poolSize = 10;

    [Header("Zona central protegida")]
    [Tooltip("Radio en píxeles alrededor del centro donde NO aparecen objetos (donde está el premio)")]
    [SerializeField] private float centerProtectedRadius = 150f;

    [Header("Márgenes del área")]
    [Tooltip("Margen desde los bordes de la pantalla")]
    [SerializeField] private float edgeMargin = 80f;

    [Tooltip("Distancia mínima entre objetos del mismo grupo")]
    [SerializeField] private float minDistanceBetweenObjects = 120f;

    [Header("Dificultad de Animación")]
    [Tooltip("Multiplicador base de la velocidad de animación")]
    [SerializeField] private float animationSpeedMultiplier = 1f;
    
    [Tooltip("Cuánto aumenta la velocidad de animación por cada acierto")]
    [SerializeField] private float speedIncreasePerHit = 0.05f;
    
    [Tooltip("Límite máximo de velocidad (para que la animación no se rompa)")]
    [SerializeField] private float maxSpeedMultiplier = 3f;

    // ─────────────────────────────────────────────
    //  Estado interno
    // ─────────────────────────────────────────────
    private List<PulseReflexObject> objectPool = new List<PulseReflexObject>();
    private List<PulseReflexObject> activeObjects = new List<PulseReflexObject>();
    private bool isSpawning = false;
    private Coroutine spawnCoroutine;
    private PulseReflexConfig currentConfig;

    // ─────────────────────────────────────────────
    //  Inicialización
    // ─────────────────────────────────────────────
    private void Awake()
    {
        InitPool();
    }

    private void InitPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(objectPrefab, spawnArea);
            PulseReflexObject pulseObj = obj.GetComponent<PulseReflexObject>();
            pulseObj.Init(this);
            obj.SetActive(false);
            objectPool.Add(pulseObj);
        }
        Debug.Log($"[Spawner] Pool inicializado con {poolSize} objetos");
    }

    // ─────────────────────────────────────────────
    //  White Label
    // ─────────────────────────────────────────────
    public void ApplyBrand(PulseReflexConfig config)
    {
        currentConfig = config;
    }

    // ─────────────────────────────────────────────
    //  Control de spawneo
    // ─────────────────────────────────────────────
    public void StartSpawning()
    {
        isSpawning = true;
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        spawnCoroutine = StartCoroutine(SpawnLoop());
        Debug.Log("[Spawner] Spawning iniciado");
    }

    public void StopSpawning()
    {
        isSpawning = false;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        ReturnAllToPool();
    }

    // ─────────────────────────────────────────────
    //  Loop principal
    //  Cada iteración: spawnea un grupo, espera a que
    //  termine su tiempo visible + intervalo, repite.
    //  La dificultad viene del GameManager en tiempo real.
    // ─────────────────────────────────────────────
    private IEnumerator SpawnLoop()
    {
        while (isSpawning)
        {
            int groupSize = gameManager.CurrentGroupSize;
            float visibleDuration = gameManager.CurrentVisibleDuration;

            SpawnGroup(groupSize, visibleDuration);

            // Esperar duración + intervalo antes del siguiente grupo
            yield return new WaitForSeconds(visibleDuration + gameManager.SpawnInterval);
        }
    }

    // ─────────────────────────────────────────────
    //  Spawneo de grupo
    // ─────────────────────────────────────────────
    private void SpawnGroup(int count, float visibleDuration)
    {
        List<Vector2> usedPositions = new List<Vector2>();

        for (int i = 0; i < count; i++)
        {
            PulseReflexObject obj = GetFromPool();
            if (obj == null)
            {
                Debug.LogWarning("[Spawner] Pool agotado — aumenta Pool Size en el Inspector");
                break;
            }

            Vector2 pos = GetValidPosition(usedPositions);
            usedPositions.Add(pos);

            Sprite sprite = GetRandomSprite();
            obj.Activate(pos, visibleDuration, sprite, animationSpeedMultiplier);
            activeObjects.Add(obj);
        }

        Debug.Log($"[Spawner] Grupo: {count} objetos | duración: {visibleDuration:F1}s");
    }

    // ─────────────────────────────────────────────
    //  Posición válida:
    //  - Dentro del área con margen desde bordes
    //  - Fuera del radio central protegido (donde está el premio)
    //  - Sin solaparse con otros objetos del grupo
    // ─────────────────────────────────────────────
    private Vector2 GetValidPosition(List<Vector2> usedPositions)
    {
        Rect rect = spawnArea.rect;
        float xMin = rect.xMin + edgeMargin;
        float xMax = rect.xMax - edgeMargin;
        float yMin = rect.yMin + edgeMargin;
        float yMax = rect.yMax - edgeMargin;

        Vector2 candidate = Vector2.zero;
        int maxAttempts = 30;
        bool found = false;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            candidate = new Vector2(
                Random.Range(xMin, xMax),
                Random.Range(yMin, yMax)
            );

            if (candidate.magnitude < centerProtectedRadius)
                continue;

            bool tooClose = false;
            foreach (Vector2 used in usedPositions)
            {
                if (Vector2.Distance(candidate, used) < minDistanceBetweenObjects)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                found = true;
                break;
            }
        }

        if (!found)
            Debug.LogWarning($"[Spawner] No se encontró posición válida en {maxAttempts} intentos — revisa Center Protected Radius o Edge Margin");

        return candidate;
    }

    private Sprite GetRandomSprite()
    {
        if (currentConfig == null ||
            currentConfig.spawnableSprites == null ||
            currentConfig.spawnableSprites.Length == 0)
            return null;

        return currentConfig.spawnableSprites[
            Random.Range(0, currentConfig.spawnableSprites.Length)
        ];
    }

    // ─────────────────────────────────────────────
    //  Object Pool
    // ─────────────────────────────────────────────
    private PulseReflexObject GetFromPool()
    {
        foreach (var obj in objectPool)
            if (!obj.gameObject.activeSelf)
                return obj;
        return null;
    }

    public void ReturnToPool(PulseReflexObject obj)
    {
        activeObjects.Remove(obj);
        obj.gameObject.SetActive(false);
    }

    private void ReturnAllToPool()
    {
        foreach (var obj in activeObjects)
            if (obj != null) obj.gameObject.SetActive(false);
        activeObjects.Clear();
    }

    // ─────────────────────────────────────────────
    //  Callbacks desde PulseReflexObject
    // ─────────────────────────────────────────────
    public void OnObjectHit(PulseReflexObject obj)
    {
        ReturnToPool(obj);
        
        // NUEVO: Aumentar la velocidad de animación y limitarla al máximo permitido
        animationSpeedMultiplier += speedIncreasePerHit;
        if (animationSpeedMultiplier > maxSpeedMultiplier)
        {
            animationSpeedMultiplier = maxSpeedMultiplier;
        }

        gameManager.RegisterHit();
    }

    public void OnObjectExpired(PulseReflexObject obj)
    {
        ReturnToPool(obj);
    }
}