using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Controla toda la UI de Pulse Reflex.
///
/// Referencias a asignar en Inspector:
/// - timerText: cronómetro
/// - hitCountText: "X / 12 aciertos"
/// - prizeAssemblyImage: Image del premio central (fill amount = progreso)
/// - prizeAssemblyContainer: GameObject padre del premio
/// - winScreen / loseScreen: paneles de resultado
/// </summary>
public class PulseReflexUI : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text hitCountText;

    [Header("Premio central")]
    [SerializeField] private GameObject prizeAssemblyContainer;
    [SerializeField] private Image prizeAssemblyImage;
    [Tooltip("Imagen de fondo/silueta del premio (siempre visible)")]
    [SerializeField] private Image prizeSilhouette;

    [Header("Pantalla de victoria")]
    [SerializeField] private GameObject winScreen;
    [SerializeField] private Image winPrizeImage;
    [SerializeField] private TMP_Text winPrizeNameText;
    [SerializeField] private TMP_Text winPrizeDescText;

    [Header("Pantalla de derrota")]
    [SerializeField] private GameObject loseScreen;

    // ─────────────────────────────────────────────
    //  Inicialización
    // ─────────────────────────────────────────────
    private void Awake()
    {
        winScreen?.SetActive(false);
        loseScreen?.SetActive(false);

        if (prizeAssemblyImage != null)
            prizeAssemblyImage.fillAmount = 0f;
    }

    // ─────────────────────────────────────────────
    //  White Label
    // ─────────────────────────────────────────────
    public void ApplyBrand(PulseReflexConfig config)
    {
        if (config == null) return;

        if (prizeAssemblyImage != null && config.prizeSprite != null)
            prizeAssemblyImage.sprite = config.prizeSprite;

        if (prizeSilhouette != null && config.prizeSprite != null)
        {
            prizeSilhouette.sprite = config.prizeSprite;
            prizeSilhouette.color = new Color(0.2f, 0.2f, 0.2f, 0.4f); // silueta oscura
        }
    }

    // ─────────────────────────────────────────────
    //  Actualización general
    // ─────────────────────────────────────────────
    public void UpdateAll(PulseReflexManager manager)
    {
        UpdateTimer(manager.RemainingTime);
        UpdateHitCount(manager.HitsLanded, manager.HitsToWin);
        UpdatePrizeAssembly(0f);
    }

    // ─────────────────────────────────────────────
    //  Timer
    // ─────────────────────────────────────────────
    public void UpdateTimer(float seconds)
    {
        if (timerText == null) return;
        seconds = Mathf.Max(0f, seconds);
        int min = Mathf.FloorToInt(seconds / 60f);
        int sec = Mathf.FloorToInt(seconds % 60f);
        timerText.text = $"{min}:{sec:00}";
        timerText.color = seconds <= 15f ? Color.red : Color.white;
    }

    // ─────────────────────────────────────────────
    //  Contador de aciertos
    // ─────────────────────────────────────────────
    public void UpdateHitCount(int current, int total)
    {
        if (hitCountText != null)
            hitCountText.text = $"{current} / {total}";

        Debug.Log($"[UI] HitCount actualizado: {current}/{total}");
    }

    // ─────────────────────────────────────────────
    //  Ensamblado del premio central
    //  Usa fillAmount para revelar la imagen progresivamente
    // ─────────────────────────────────────────────
    public void UpdatePrizeAssembly(float progress)
    {
        if (prizeAssemblyImage != null)
            prizeAssemblyImage.fillAmount = progress;
    }

    // ─────────────────────────────────────────────
    //  Pantallas de resultado
    // ─────────────────────────────────────────────
    public void ShowWinScreen(PulseReflexConfig config)
    {
        if (winScreen == null) return;
        winScreen.SetActive(true);

        if (config != null)
        {
            if (winPrizeImage != null && config.prizeSprite != null)
                winPrizeImage.sprite = config.prizeSprite;
            if (winPrizeNameText != null)
                winPrizeNameText.text = config.prizeName;
            if (winPrizeDescText != null)
                winPrizeDescText.text = config.prizeDescription;
        }
    }

    public void ShowLoseScreen()
    {
        if (loseScreen == null)
        {
            Debug.LogWarning("[UI] LoseScreen no asignado en Inspector");
            return;
        }
        loseScreen.SetActive(true);
        Debug.Log("[UI] LoseScreen activado");
    }
}
