using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [Header("Cámaras Virtuales")]
    public CinemachineVirtualCamera cam1ra;
    public CinemachineVirtualCamera cam3ra;
    public CinemachineVirtualCamera cam25D;
    public CinemachineVirtualCamera camIso;

    [Header("Configuración de Control")]
    public int vistaActual = 1; 

    private void Start()
    {
        CambiarVista(vistaActual);
    }

    private void Update()
    {
        // Cambiamos a la forma moderna de detectar teclas para evitar errores rojos
        if (Keyboard.current.digit1Key.wasPressedThisFrame) CambiarVista(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) CambiarVista(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) CambiarVista(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) CambiarVista(3);
    }

    public void CambiarVista(int indice)
    {
        vistaActual = indice;

        // Reset de prioridades
        cam1ra.Priority = 10;
        cam3ra.Priority = 10;
        cam25D.Priority = 10;
        camIso.Priority = 10;

        switch (indice)
        {
            case 0: cam1ra.Priority = 20; break;
            case 1: cam3ra.Priority = 20; break;
            case 2: cam25D.Priority = 20; break;
            case 3: camIso.Priority = 20; break;
        }

        ActualizarVisibilidadPersonaje(indice);
        Debug.Log("Cámara activa: " + indice);
    }

    void ActualizarVisibilidadPersonaje(int indice)
    {
        int capaInvisible = LayerMask.NameToLayer("CuerpoInvisible");
        if (capaInvisible == -1) return; // Por si el nombre está mal escrito

        if (indice == 0) // 1ra Persona
            Camera.main.cullingMask &= ~(1 << capaInvisible);
        else // 3ra, 2.5D, ISO
            Camera.main.cullingMask |= (1 << capaInvisible);
    }
}