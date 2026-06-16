using UnityEngine;

public class InteraccionCostal : MonoBehaviour, IInteractable
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí el objeto que tiene tu BrandImpactGameManager")]
    [SerializeField] private BrandImpactGameManager gameManager;

    [Header("Textos de Interacción")]
    [SerializeField] private string textoInteraccion = "Presiona [E] para entrenar";

    [Header("Efecto Visual (Halo Glint)")]
    [Tooltip("Arrastra aquí el objeto de tu Particle System (el destello)")]
    [SerializeField] private GameObject luzDestello;

    public void Interact()
    {
        // Si el juego está en estado Idle, lo iniciamos
        if (gameManager != null && gameManager.CurrentState == BrandImpactGameManager.GameState.Idle)
        {
            gameManager.StartGame();
            
            // Apagamos la luz definitivamente cuando el minijuego comience
            if (luzDestello != null) luzDestello.SetActive(false);
        }
    }

    public void OnFocus()
    {
        // Cuando el rayo del jugador toca el costal (lo estás viendo de cerca), apagamos el destello
        if (luzDestello != null) luzDestello.SetActive(false);
    }

    public void OnLoseFocus()
    {
        // Si dejas de mirar el costal Y el juego aún no empieza, volvemos a prender la luz
        if (luzDestello != null && gameManager != null && gameManager.CurrentState == BrandImpactGameManager.GameState.Idle)
        {
            luzDestello.SetActive(true);
        }
    }

    public string GetInteractionText()
    {
        // Si el juego ya empezó, ocultamos el texto para que no estorbe en la pantalla
        if (gameManager != null && gameManager.CurrentState != BrandImpactGameManager.GameState.Idle)
        {
            return ""; 
        }
        
        // Si el juego no ha empezado, mostramos el mensaje
        return textoInteraccion;
    }
}