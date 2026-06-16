using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BrandImpactUI : MonoBehaviour
{
    [Header("HUD principal")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text punchCountText;

    [Header("Barra de hold")]
    [SerializeField] private GameObject holdBarContainer;
    [SerializeField] private Image holdBarFill;
    [SerializeField] private Image holdBarGlow;

    [Header("Indicador de Flecha Dinámica (NUEVO)")]
    [SerializeField] private RectTransform arrowContainer;
    [SerializeField] private TMP_Text arrowTimeText;
    [Tooltip("El valor de 'Pos Y' de la flecha en el punto más bajo")]
    [SerializeField] private float barMinY; 
    [Tooltip("El valor de 'Pos Y' de la flecha en el punto más alto")]
    [SerializeField] private float barMaxY;
    [Tooltip("Segundos que tarda en llenarse la barra (para calcular el texto)")]
    [SerializeField] private float maxHoldTime = 3f; 

    // Se eliminaron por completo las variables de Feedback (Super Punch e Invalid Punch)

    [Header("Pantalla de victoria")]
    [SerializeField] private GameObject winScreen;
    [SerializeField] private Image prizeImage;
    [SerializeField] private TMP_Text prizeNameText;
    [SerializeField] private TMP_Text prizeDescriptionText;

    [Header("Pantalla de derrota")]
    [SerializeField] private GameObject loseScreen;

    [Header("Colores de la barra")]
    [Tooltip("Configura los colores desde frío (0) hasta caliente (1)")]
    [SerializeField] private Gradient colorEnergia;

    private void Awake()
    {
        winScreen?.SetActive(false);
        loseScreen?.SetActive(false);
        holdBarContainer?.SetActive(false);

        if (holdBarFill != null)
            holdBarFill.fillAmount = 0f;
    }

    public void ApplyBrand(BrandImpactConfig config)
    {
        if (config == null) return;
    }

    public void UpdateAll(BrandImpactGameManager manager)
    {
        UpdateTimer(manager.RemainingTime);
        UpdatePunchCount(manager.PunchesLanded, manager.TotalPunches);
        ResetHoldBar();
    }

    // ── Timer ──────────────────────────────────────
    public void UpdateTimer(float seconds)
    {
        if (timerText == null) return;

        seconds = Mathf.Max(0f, seconds);
        int min = Mathf.FloorToInt(seconds / 60f);
        int sec = Mathf.FloorToInt(seconds % 60f);
        timerText.text = $"{min}:{sec:00}";
        timerText.color = seconds <= 15f ? Color.red : Color.white;
    }

    // ── Contador de golpes ─────────────────────────
    public void UpdatePunchCount(int current, int total)
    {
        if (punchCountText != null)
        {
            punchCountText.text = $"{current} / {total}";
        }
    }

    // ── Barra de hold ──────────────────────────────
    public void ShowHoldBar()
    {
        holdBarContainer?.SetActive(true);
        if (holdBarFill != null)
        {
            holdBarFill.fillAmount = 0f;
            holdBarFill.color = Color.white;
        }
    }

    public void UpdateHoldBar(float progress)
    {
        if (holdBarFill != null)
        {
            holdBarFill.fillAmount = progress;
        }

        if (arrowContainer != null)
        {
            float currentY = Mathf.Lerp(barMinY, barMaxY, progress);
            arrowContainer.anchoredPosition = new Vector2(arrowContainer.anchoredPosition.x, currentY);

            if (arrowTimeText != null)
            {
                float currentSeconds = progress * maxHoldTime;
                arrowTimeText.text = currentSeconds.ToString("F1") + "s";
            }
        }
    }

    public void ShowSuperPunchFeedback()
    {
        // Solo activamos el brillo de la barra si existe
        if (holdBarGlow != null)
            StartCoroutine(GlowPulse());
    }

    public void ResetHoldBar()
    {
        holdBarContainer?.SetActive(false);
        StopCoroutine(nameof(GlowPulse));

        if (holdBarFill != null)
        {
            holdBarFill.fillAmount = 0f;
            holdBarFill.color = Color.white;
        }
        if (holdBarGlow != null)
        {
            holdBarGlow.color = new Color(1f, 1f, 1f, 0f);
        }

        if (arrowContainer != null)
        {
            arrowContainer.anchoredPosition = new Vector2(arrowContainer.anchoredPosition.x, barMinY);
        }
        if (arrowTimeText != null)
        {
            arrowTimeText.text = "0.0s";
        }
    }

    // ── Feedback inválido ──────────────────────────
    public void ShowInvalidPunchFeedback()
    {
        // Se mantiene el método vacío para que el GameManager no arroje errores
    }

    // ── Pantallas de resultado ─────────────────────
    public void ShowWinScreen(BrandImpactConfig config)
    {
        if (winScreen == null) return;
        winScreen.SetActive(true);

        if (config != null)
        {
            if (prizeImage != null && config.prizeSprite != null)
                prizeImage.sprite = config.prizeSprite;
            if (prizeNameText != null)
                prizeNameText.text = config.prizeName;
            if (prizeDescriptionText != null)
                prizeDescriptionText.text = config.prizeDescription;
        }
    }

    public void ShowLoseScreen()
    {
        if (loseScreen == null) return;
        loseScreen.SetActive(true);
    }

    // ── Corrutinas ─────────────────────────────────
    private IEnumerator ShowAndHide(GameObject target, float delay)
    {
        target.SetActive(true);
        yield return new WaitForSeconds(delay);
        target.SetActive(false);
    }

    private IEnumerator GlowPulse()
    {
        if (holdBarGlow == null) yield break;
        float t = 0f;
        while (true)
        {
            t += Time.deltaTime * 4f;
            holdBarGlow.color = new Color(1f, 1f, 1f, Mathf.Sin(t) * 0.5f + 0.5f);
            yield return null;
        }
    }
}