using UnityEngine;
using UnityEngine.XR;

public class DetectorPlataforma : MonoBehaviour
{
    [Header("Herramientas de Prueba")]
    [Tooltip("Marca esto para que Unity finja ser un celular mientras editas")]
    public bool simularMovilEnPC = true;

    void Start()
    {
        // 1. Apagar si hay VR
        if (XRSettings.isDeviceActive)
        {
            gameObject.SetActive(false);
            return; 
        }

        // 2. Revisamos si es celular físicamente
        bool esMovil = SystemInfo.deviceType == DeviceType.Handheld;

        // --- MAGIA PARA PRUEBAS EN PC ---
        // Si estamos jugando dentro del Editor de Unity, ignoramos el hardware
        // y le hacemos caso a tu casilla 'simularMovilEnPC'
#if UNITY_EDITOR
        esMovil = simularMovilEnPC;
#endif

        // 3. Encender o apagar
        if (esMovil)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}