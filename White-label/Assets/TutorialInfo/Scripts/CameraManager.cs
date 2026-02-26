using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [Header("Cámaras Virtuales")]
    public CinemachineVirtualCamera cam1ra;
    public CinemachineVirtualCamera cam3ra;
    public CinemachineVirtualCamera cam25D;
    public CinemachineVirtualCamera camIso;

    [Header("Configuración de Control")]
    [Tooltip("Índice de la vista inicial (0:1P, 1:3P, 2:2.5D, 3:Iso)")]
    public int vistaActual = 1; 

    void Start()
    {
        // Al iniciar, nos aseguramos de que la cámara correcta tenga la prioridad
        CambiarVista(vistaActual);
    }

    void Update()
    {
        // Teclas rápidas para probar el cambio de cámara durante el juego
        if (Input.GetKeyDown(KeyCode.Alpha1)) CambiarVista(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) CambiarVista(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) CambiarVista(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) CambiarVista(3);
    }

    public void CambiarVista(int indice)
    {
        vistaActual = indice;

        // Reset de prioridades (todas vuelven a la base de 10)
        cam1ra.Priority = 10;
        cam3ra.Priority = 10;
        cam25D.Priority = 10;
        camIso.Priority = 10;

        // La cámara seleccionada sube su prioridad para que Cinemachine haga el cambio
        switch (indice)
        {
            case 0: cam1ra.Priority = 20; break;
            case 1: cam3ra.Priority = 20; break;
            case 2: cam25D.Priority = 20; break;
            case 3: camIso.Priority = 20; break;
        }

        Debug.Log("Cámara activa: " + indice);
    }
}