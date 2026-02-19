using UnityEngine;

public class ObjetoInteractivo : MonoBehaviour, IInteractable
{
    [Header("Configuración Visual")]
    public Color highlightColor = Color.yellow; // Color cuando el jugador mira el objeto

    private Color originalColor;
    private Renderer meshRenderer;
    private bool isFocused = false;

    private void Awake()
    {
        // Obtenemos el renderizador para cambiar el material/color
        meshRenderer = GetComponent<Renderer>();

        if (meshRenderer != null)
        {
            originalColor = meshRenderer.material.color;
        }
    }

    // Tarea: Sistema de interacción genérico
    public void Interact()
    {
        // Aquí defines qué hace el objeto (ej. abrir una puerta empresarial o activar un sensor de bienestar)
        Debug.Log("¡Interacción exitosa con: " + gameObject.name + "!");

        // Ejemplo: Cambiar a un color aleatorio al interactuar
        meshRenderer.material.color = Random.ColorHSV();
        originalColor = meshRenderer.material.color; 
    }

    // Tarea: Sistema de feedback (Highlight y Prompt)
    public void OnFocus()
    {
        if (isFocused) return;

        isFocused = true;

        // 1. Efecto Visual (Highlight)
        if (meshRenderer != null)
        {
            meshRenderer.material.color = highlightColor;
        }

        // 2. Mostrar Texto (Prompt)
        if (InteractionManager.Instance != null)
        {
            InteractionManager.Instance.ShowPrompt(true);
        }
    }

    public void OnLoseFocus()
    {
        if (!isFocused) return;

        isFocused = false;

        // 1. Restaurar Color Original
        if (meshRenderer != null)
        {
            meshRenderer.material.color = originalColor;
        }

        // 2. Ocultar Texto (Prompt)
        if (InteractionManager.Instance != null)
        {
            InteractionManager.Instance.ShowPrompt(false);
        }
    }
}