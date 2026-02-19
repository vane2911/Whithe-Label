public interface IInteractable
{
    void Interact();
    void OnFocus();   // Se activa cuando el Raycast lo detecta
    void OnLoseFocus(); // Se activa cuando dejas de mirarlo
}