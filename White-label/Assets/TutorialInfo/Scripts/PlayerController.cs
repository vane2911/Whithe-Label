using UnityEngine;
using UnityEngine.InputSystem; // Requerido para el Input System unificado

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float playerSpeed = 5.0f;
    public float gravityValue = -9.81f;

    [Header("Configuración de Interacción")]
    public float interactDistance = 3.0f; // Distancia máxima para interactuar
   
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
   
    // Almacenamiento de Input
    private Vector2 moveInput;
    private IInteractable currentlyFocusedInteractable; // Para el sistema de feedback

    private void Start()
    {
        controller = GetComponent<CharacterController>();
       
        // Configuración de visualización del cursor para la Fase 2
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Tarea: Implementar movimiento básico del jugador (Teclado y Gamepad)
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Tarea: Sistema de interacción genérico (Activado por 'E' o 'Button South')
    public void OnInteract(InputValue value)
    {
        if (value.isPressed && currentlyFocusedInteractable != null)
        {
            currentlyFocusedInteractable.Interact();
            Debug.Log("Interactuando con objeto");
        }
    }

    void Update()
    {
        // 1. Lógica de Gravedad
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        // 2. Lógica de Movimiento relativo a la cámara
        Vector3 move = Camera.main.transform.forward * moveInput.y + Camera.main.transform.right * moveInput.x;
        move.y = 0f; // Bloqueamos el eje Y para evitar que el jugador vuele al mirar arriba

        controller.Move(move * Time.deltaTime * playerSpeed);

        // Aplicar caída por gravedad
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // 3. Tarea: Sistema de feedback (Detección constante del objetivo)
        HandleInteractionDetection();
    }

    private void HandleInteractionDetection()
    {
        // Lanzamos un rayo desde el centro de la cámara hacia adelante
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            // Intentamos obtener el componente interactuable
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                if (currentlyFocusedInteractable != interactable)
                {
                    // Si cambiamos de objeto, limpiamos el anterior
                    if (currentlyFocusedInteractable != null) currentlyFocusedInteractable.OnLoseFocus();
                   
                    // Activamos el feedback visual en el nuevo objeto
                    currentlyFocusedInteractable = interactable;
                    currentlyFocusedInteractable.OnFocus();
                }
            }
            else
            {
                ClearCurrentFocus();
            }
        }
        else
        {
            ClearCurrentFocus();
        }
    }

    private void ClearCurrentFocus()
    {
        if (currentlyFocusedInteractable != null)
        {
            currentlyFocusedInteractable.OnLoseFocus();
            currentlyFocusedInteractable = null;
        }
    }
}