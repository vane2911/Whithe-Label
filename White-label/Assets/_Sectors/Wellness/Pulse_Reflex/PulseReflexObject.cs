using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Representa un objeto tocable individual en Pulse Reflex.
/// Se activa, espera a ser tocado o expira, y regresa al pool.
///
/// Setup del prefab:
/// - GameObject con Image (UI) para mostrar el sprite
/// - Agregar Button o usar OnPointerDown para detectar el toque
/// - Agregar este script
/// </summary>
public class PulseReflexObject : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] private Image objectImage;
    [SerializeField] private RectTransform rectTransform;

    // ─────────────────────────────────────────────
    //  Estado interno
    // ─────────────────────────────────────────────
    private PulseReflexSpawner spawner;
    private float visibleDuration;
    private float timer;
    private bool isActive = false;
    private bool wasHit = false;
    private Coroutine lifeCoroutine;
    private float currentAnimSpeed = 1f;

    // ─────────────────────────────────────────────
    //  Inicialización (llamado por el pool)
    // ─────────────────────────────────────────────
    public void Init(PulseReflexSpawner spawnerRef)
    {
        spawner = spawnerRef;
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        if (objectImage == null)
            objectImage = GetComponent<Image>();
    }

    // ─────────────────────────────────────────────
    //  Activación
    // ─────────────────────────────────────────────
    public void Activate(Vector2 anchoredPosition, float duration, Sprite sprite, float speedMultiplier = 1f)
    {
        wasHit = false;
        isActive = true;
        visibleDuration = duration;
        timer = 0f;
        
        currentAnimSpeed = speedMultiplier;

        rectTransform.anchoredPosition = anchoredPosition;

        if (objectImage != null && sprite != null)
            objectImage.sprite = sprite;

        // Color base visible
        if (objectImage != null)
            objectImage.color = Color.white;

        gameObject.SetActive(true);

        // Animación de entrada
        if (lifeCoroutine != null) StopCoroutine(lifeCoroutine);
        lifeCoroutine = StartCoroutine(LifeCycle());

        Debug.Log($"[PulseReflexObject] Activado en {anchoredPosition} por {duration}s");
    }

    // ─────────────────────────────────────────────
    //  Ciclo de vida
    // ─────────────────────────────────────────────
    private IEnumerator LifeCycle()
    {
        // Animación de entrada: escala de 0 a 1
        yield return StartCoroutine(ScaleAnimation(Vector3.zero, Vector3.one, 0.15f / currentAnimSpeed));

        // Esperar duración visible
        float elapsed = 0f;
        while (elapsed < visibleDuration && isActive)
        {
            elapsed += Time.deltaTime;

            // Parpadeo en el último 30% del tiempo para avisar que va a desaparecer
            float timeLeft = visibleDuration - elapsed;
            if (timeLeft < visibleDuration * 0.3f)
            {
                float blink = Mathf.Sin(elapsed * 20f) * 0.5f + 0.5f;
                if (objectImage != null)
                    objectImage.color = new Color(1f, 1f, 1f, Mathf.Lerp(0.4f, 1f, blink));
            }

            yield return null;
        }

        if (isActive && !wasHit)
        {
            // Animación de salida: escala de 1 a 0 (dividimos el tiempo)
            yield return StartCoroutine(ScaleAnimation(Vector3.one, Vector3.zero, 0.12f / currentAnimSpeed));
            spawner?.OnObjectExpired(this);
        }
    }

    // ─────────────────────────────────────────────
    //  Input — detectar toque/clic
    // ─────────────────────────────────────────────

    /// <summary>
    /// Llamar desde un Button (OnClick) o desde un EventTrigger (PointerDown).
    /// </summary>
    public void OnTouched()
    {
        if (!isActive || wasHit) return;

        wasHit = true;
        isActive = false;

        if (lifeCoroutine != null) StopCoroutine(lifeCoroutine);
        StartCoroutine(HitAnimation());

        Debug.Log("[PulseReflexObject] ¡Tocado!");
    }

    // ─────────────────────────────────────────────
    //  Animaciones
    // ─────────────────────────────────────────────
    private IEnumerator HitAnimation()
    {
        // Flash verde y escala hacia arriba luego desaparece
        if (objectImage != null)
            objectImage.color = Color.green;

        yield return StartCoroutine(ScaleAnimation(Vector3.one, Vector3.one * 1.3f, 0.08f / currentAnimSpeed));
        yield return StartCoroutine(ScaleAnimation(Vector3.one * 1.3f, Vector3.zero, 0.1f / currentAnimSpeed));

        spawner?.OnObjectHit(this);
    }

    private IEnumerator ScaleAnimation(Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        transform.localScale = to;
    }

    // ─────────────────────────────────────────────
    //  Reset al volver al pool
    // ─────────────────────────────────────────────
    private void OnDisable()
    {
        isActive = false;
        wasHit = false;
        transform.localScale = Vector3.one;
        if (objectImage != null)
            objectImage.color = Color.white;
    }
}