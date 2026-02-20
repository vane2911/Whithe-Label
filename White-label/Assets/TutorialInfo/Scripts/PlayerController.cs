using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float playerSpeed = 5.0f;
    public float gravityValue = -9.81f;
    
    [Header("Configuración de Cámara (Touchpad)")]
    public Transform eyesTransform;
    public float sensitivity = 0.15f;      
    private float xRotation = 0f;

    [Header("Configuración de Interacción")]
    public float interactionDistance = 6.0f;
    public LayerMask interactableLayer;
    public GameObject interactionContainer;
    public TextMeshProUGUI mensajeTexto;

    [Header("Referencia al PopUp (Onboarding)")]
    public GameObject popUpBienvenida; // Arrastra aquí el panel "PopUp_Bienvenida"

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 playerVelocity;
    private IInteractable currentInteractable;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        
        // Al inicio, si el PopUp está activo, liberamos el ratón para poder cerrarlo
        ActualizarEstadoCursor();

        if (interactionContainer != null) interactionContainer.SetActive(false);
    }

    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    public void OnLook(InputValue value) => lookInput = value.Get<Vector2>();
    
    public void OnInteract(InputValue value)
    {
        // Solo interactúa si no hay Pop-up estorbando
        if (popUpBienvenida != null && popUpBienvenida.activeSelf) return;

        if (currentInteractable != null && value.isPressed) 
        {
            currentInteractable.Interact();
        }
    }

    void Update()
    {
        // PASO CLAVE: Si el PopUp está activo, detenemos todo y liberamos el cursor
        if (popUpBienvenida != null && popUpBienvenida.activeSelf)
        {
            ActualizarEstadoCursor();
            return; 
        }

        // Si llegamos aquí, el PopUp está cerrado, así que bloqueamos el cursor y movemos
        ActualizarEstadoCursor();
        ManejarRotacion();
        ManejarMovimiento();
        ManejarInteraccion();
    }

    void ActualizarEstadoCursor()
    {
        if (popUpBienvenida != null && popUpBienvenida.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None; // Permite mover el ratón para picar botones
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // Bloquea el ratón al centro para jugar
            Cursor.visible = false;
        }
    }

    void ManejarRotacion()
    {
        float mouseX = lookInput.x * sensitivity;
        float mouseY = lookInput.y * sensitivity;

        transform.Rotate(Vector3.up * mouseX); 

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f); 
        
        if (eyesTransform != null)
            eyesTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void ManejarMovimiento()
    {
        if (controller.isGrounded && playerVelocity.y < 0) playerVelocity.y = 0f;

        Vector3 move = transform.forward * moveInput.y + transform.right * moveInput.x;
        controller.Move(move * Time.deltaTime * playerSpeed);

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    void ManejarInteraccion()
    {
        if (eyesTransform == null) return;
        Ray ray = new Ray(eyesTransform.position, eyesTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            
            if (interactable != null)
            {
                if (interactable != currentInteractable)
                {
                    if (currentInteractable != null) currentInteractable.OnLoseFocus();
                    
                    currentInteractable = interactable;
                    currentInteractable.OnFocus(); 

                    if (interactionContainer != null) 
                    {
                        interactionContainer.SetActive(true);
                        if (mensajeTexto != null) mensajeTexto.text = "Presiona E para interactuar"; 
                    }
                }
            }
            else { LimpiarInteraccion(); }
        }
        else { LimpiarInteraccion(); }
    }

    void LimpiarInteraccion()
    {
        if (currentInteractable != null)
        {
            currentInteractable.OnLoseFocus();
            currentInteractable = null;
            if (interactionContainer != null) interactionContainer.SetActive(false);
        }
    }
}