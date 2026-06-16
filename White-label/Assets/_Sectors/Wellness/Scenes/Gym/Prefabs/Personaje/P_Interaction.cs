using UnityEngine;
using UnityEngine.InputSystem;

namespace StarterAssets
{
    public class P_Interaction : MonoBehaviour
    {
        [Header("Configuración de Interacción")]
        public float interactionDistance = 1.5f; 
        public LayerMask interactableLayer;

        [Header("Referencias de UI")]
        public GameObject interactionUI; 
        public TMPro.TextMeshProUGUI interactionText;

        private IInteractable _currentInteractable;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;
        private PlayerInput _playerInput;
        private bool _teclaLiberada = true; 

        private void Awake()
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            _input = GetComponent<StarterAssetsInputs>();
            _playerInput = GetComponent<PlayerInput>();
            
            if (interactionUI != null) interactionUI.SetActive(false);
        }

        private void Update()
        {
            ManejarInteraccion();
            
            if (_input != null && !_input.interact)
            {
                _teclaLiberada = true;
            }
        }

        private void ManejarInteraccion()
        {
            Ray ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);
            
            if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactableLayer))
            {
                IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
                
                if (interactable != null)
                {
                    if (interactable != _currentInteractable)
                    {
                        _currentInteractable = interactable;
                        _currentInteractable.OnFocus();
                    }
                    
                    // Leemos el texto en tiempo real
                    string textoActual = _currentInteractable.GetInteractionText();

                    // LA SOLUCIÓN DEL RECUADRO: Si el texto está vacío, apagamos todo
                    if (string.IsNullOrEmpty(textoActual))
                    {
                        if (interactionUI != null) interactionUI.SetActive(false);
                    }
                    else
                    {
                        if (interactionUI != null) interactionUI.SetActive(true);
                        if (interactionText != null) interactionText.text = textoActual;
                    }
                    
                    if (_input != null && _input.interact && _teclaLiberada) 
                    {
                        _currentInteractable.Interact();
                        _teclaLiberada = false;
                        _input.interact = false; 
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
            }
            
            if (interactionUI != null) interactionUI.SetActive(false); 
        }
    }
}