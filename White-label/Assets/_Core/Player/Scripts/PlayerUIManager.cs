using UnityEngine;
using UnityEngine.InputSystem;

namespace StarterAssets
{
    public class PlayerUIManager : MonoBehaviour
    {
        [Header("Referencias de UI")]
        public GameObject popUpBienvenida;

        private StarterAssetsInputs _input;
        private PlayerInput _playerInput;

        private void Awake()
        {
            _input = GetComponent<StarterAssetsInputs>();
            _playerInput = GetComponent<PlayerInput>();
        }

        private void Update()
        {
            ControlarEstadoJuego();
        }

        private void ControlarEstadoJuego()
        {
            // Verificamos si el PopUp está activo
            bool uiActiva = popUpBienvenida != null && popUpBienvenida.activeSelf;

            if (uiActiva)
            {
                // 1. Liberamos el mouse para interactuar con el botón
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                // 2. "Engañamos" al sistema de input para que el personaje no se mueva
                _input.move = Vector2.zero;
                _input.look = Vector2.zero;
                _input.jump = false;
                _input.sprint = false;
            }
            else
            {
                // Si la UI está cerrada, bloqueamos el mouse para jugar
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        // Este método lo puedes llamar desde el botón "Cerrar" de tu PopUp en Unity
        public void CerrarPopUp()
        {
            if (popUpBienvenida != null)
            {
                popUpBienvenida.SetActive(false);
            }
        }
    }
}