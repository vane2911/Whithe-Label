using UnityEngine;
using UnityEngine.InputSystem;

namespace StarterAssets
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Configuración de Interacción")]
        [Tooltip("Distancia máxima para interactuar con objetos")]
        public float interactionDistance = 5.0f;
        
        [Tooltip("Capa (Layer) donde están los objetos interactuables")]
        public LayerMask interactableLayer;

        [Header("Referencias de UI (Opcional)")]
        public GameObject interactionUI; 
        public TMPro.TextMeshProUGUI interactionText;

        private IInteractable _currentInteractable;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;
        private PlayerInput _playerInput;

        private void Awake()
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            _input = GetComponent<StarterAssetsInputs>();
            _playerInput = GetComponent<PlayerInput>();
        }

        private void Update()
        {
            ManejarInteraccion();
        }

        private void ManejarInteraccion()
        {
            // El rayo sale del centro de la pantalla (cámara) hacia adelante
            Ray ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);
            
            if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactableLayer))
            {
                // Buscamos la interfaz en el objeto golpeado o sus padres
                IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
                
                if (interactable != null)
                {
                    // Dentro de ManejarInteraccion, cuando detectas un objeto:
                    if (interactable != _currentInteractable)
                    {
                        _currentInteractable = interactable;
                        _currentInteractable.OnFocus();
                        
                        // Le pedimos al objeto SU propio texto
                        if (interactionText != null) 
                        {
                            interactionText.text = _currentInteractable.GetInteractionText();
                        }
                        interactionUI?.SetActive(true);
                    }
                    
                    // Si el usuario presiona el botón de interactuar
                    if (_input.interact) 
                    {
                        _currentInteractable.Interact();
                        _input.interact = false; // Reset para evitar ejecuciones múltiples
                    }
                }
                else { LimpiarInteraccion(); }
            }
            else { LimpiarInteraccion(); }
        }

        private void LimpiarInteraccion()
        {
            if (_currentInteractable != null)
            {
                _currentInteractable.OnLoseFocus();
                _currentInteractable = null;
                ActualizarTextoUI(false);
            }
        }

        private void ActualizarTextoUI(bool activo)
        {
            if (interactionUI != null) interactionUI.SetActive(activo);

            if (activo && interactionText != null)
            {
                // Detectamos el dispositivo para mostrar la tecla correcta (Marca Blanca)
                string scheme = _playerInput.currentControlScheme;
                interactionText.text = (scheme == "Gamepad") ? "Presiona [X] para interactuar" : "Presiona [E] para interactuar";
            }
        }
    }
}