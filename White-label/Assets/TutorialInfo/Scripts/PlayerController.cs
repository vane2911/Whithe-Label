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
    public Transform eyesTransform;        // Arrastra aquí el CamaraRoot
    public float sensitivity = 0.15f;      
    private float xRotation = 0f;

    [Header("Configuración de Interacción")]
    public float interactionDistance = 6.0f;
    public LayerMask interactableLayer;     // Asegúrate de que sea "Default"
    public GameObject interactionText;      // El texto de "Presiona E"

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 playerVelocity;
    private IInteractable currentInteractable;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; 
        if (interactionText != null) interactionText.SetActive(false);
    }

    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    public void OnLook(InputValue value) => lookInput = value.Get<Vector2>();
    public void OnInteract(InputValue value)
    {
        if (currentInteractable != null) currentInteractable.Interact();
    }

    void Update()
    {
        ManejarRotacion();
        ManejarMovimiento();
        ManejarInteraccion();
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

        // Lanzamos el rayo para detectar objetos en la layer Default
        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            // BUSQUEDA FLEXIBLE: Encuentra el script incluso si está en el objeto padre
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            
            if (interactable != null)
            {
                if (interactable != currentInteractable)
                {
                    if (currentInteractable != null) currentInteractable.OnLoseFocus();
                    currentInteractable = interactable;
                    currentInteractable.OnFocus(); 
                    if (interactionText != null) interactionText.SetActive(true); 
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
            if (interactionText != null) interactionText.SetActive(false);
        }
    }
}