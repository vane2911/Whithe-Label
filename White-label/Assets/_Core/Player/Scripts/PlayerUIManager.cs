using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
 
namespace StarterAssets
{
    public class PlayerUIManager : MonoBehaviour
    {
        [Header("Referencias de UI")]
        public GameObject popUpBienvenida;
 
        [Header("WhiteLabel - Eventos del PopUp")]
        [Tooltip("Se dispara cuando el PopUp se muestra")]
        public UnityEvent OnPopUpAbierto;
 
        [Tooltip("Se dispara cuando el PopUp se cierra")]
        public UnityEvent OnPopUpCerrado;
 
        private StarterAssetsInputs _input;
        private PlayerInput _playerInput;
        private bool _popUpAbiertoPrevio = false;
 
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
            bool uiActiva = popUpBienvenida != null && popUpBienvenida.activeSelf;

            GameObject carrusel = GameObject.Find("Carrusel"); // O como se llame tu objeto
            if (carrusel != null && carrusel.activeSelf) return;
 
            if (uiActiva && !_popUpAbiertoPrevio)
            {
                OnPopUpAbierto?.Invoke();
                _popUpAbiertoPrevio = true;
            }
            else if (!uiActiva && _popUpAbiertoPrevio)
            {
                OnPopUpCerrado?.Invoke();
                _popUpAbiertoPrevio = false ;
            }
 
            if (uiActiva)
            {
                _input.move = Vector2.zero;
                _input.look = Vector2.zero;
                _input.jump = false;
                _input.sprint = false;
            }
        }
 
        public void CerrarPopUp()
        {
            if (popUpBienvenida != null)
            {
                popUpBienvenida.SetActive(false);
            }
        }
    }
}