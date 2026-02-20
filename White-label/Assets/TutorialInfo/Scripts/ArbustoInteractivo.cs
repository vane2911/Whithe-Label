using UnityEngine;

public class ArbustoInteractivo : MonoBehaviour, IInteractable
{
    [Header("Referencias")]
    public ParticleSystem hojasParticulas; // Arrastra aquí tu Particle System

    public void OnFocus()
    {
        // Ya no cambiamos el tamaño, Mich. 
        // Solo se mostrará el texto de "Presiona E" definido en el PlayerController.
    }

    public void OnLoseFocus()
    {
        // No hay cambios físicos que revertir.
    }

    public void Interact()
    {
        // Esta es la única acción: disparar las hojas al presionar E
        if (hojasParticulas != null)
        {
            hojasParticulas.Play();
        }
        
        Debug.Log("Mich, has interactuado con el arbusto y soltó hojas.");
    }
}