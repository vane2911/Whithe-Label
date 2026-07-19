using UnityEngine;

// Asegúrate de que el nombre de esta clase sea igual al nombre de tu archivo
public class TelonTutorial : MonoBehaviour 
{
    [Tooltip("Velocidad a la que desaparece lo negro")]
    public float velocidadFade = 1.5f;

    private CanvasGroup grupoCanvas;

    void Awake()
    {
        // Conectamos la nueva armadura que le acabas de poner
        grupoCanvas = GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        if (grupoCanvas != null)
        {
            // El Canvas Group es inmune al bug de Unity, forzamos el negro aquí.
            grupoCanvas.alpha = 1f;
        }
    }

    void Update()
    {
        if (grupoCanvas != null && grupoCanvas.alpha > 0f)
        {
            // Restamos opacidad frame por frame
            grupoCanvas.alpha -= Time.deltaTime * velocidadFade;
        }
        else if (grupoCanvas != null && grupoCanvas.alpha <= 0f)
        {
            // Apagamos la imagen al terminar para no estorbar los clics
            gameObject.SetActive(false);
        }
    }
}