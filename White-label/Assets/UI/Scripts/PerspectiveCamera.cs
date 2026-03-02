using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiPerspectiveCamera : MonoBehaviour
{
    public enum CameraMode { FirstPerson, ThirdPerson, TwoPointFiveD, Isometric }
    
    [Header("Modo de Vista Actual")]
    public CameraMode currentMode = CameraMode.FirstPerson;

    [Header("Objetivos de Cámara (Asigna tus Empties)")]
    public Transform fpTarget;   // Los ojos del personaje
    public Transform tpTarget;   // El hombro o cabeza para 3ra persona
    public Transform twoDTarget; // Empty para la vista lateral (2.5D)
    public Transform isoTarget;  // Empty para la vista isométrica

    [Header("Visibilidad de Jugador")]
    public GameObject playerMesh;

    [Header("Ajustes de Distancia")]
    public float maxDistace = 7f;
    public float minDistance = 2f;
    public float zoomVelocity = 300f;
    public float zoomSmooth = 0.1f;
    
    [Header("Ajustes de Sensibilidad")]
    public Vector2 sensitivity = new Vector2(1.5f, 1.5f);

    [Header("Colisiones")]
    public LayerMask obstacleLayers;

    private Vector2 angle = new Vector2(90 * Mathf.Deg2Rad, 0);
    private new Camera camera;
    private Vector2 nearPlaneSize;
    private Transform currentFollow;
    private float defaultDistance;
    private float newDistance;

    void Start()
    {
        camera = GetComponent<Camera>();
        defaultDistance = (maxDistace + minDistance) / 2;
        newDistance = defaultDistance;
        
        // Configuración inicial
        ChangeView(currentMode);
        CalculateNearPlaneSize();

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        ManejarCambioVistas();
        ManejarInputsMouse();
    }

    void ManejarCambioVistas()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeView(CameraMode.FirstPerson);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeView(CameraMode.ThirdPerson);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeView(CameraMode.TwoPointFiveD);
        if (Input.GetKeyDown(KeyCode.Alpha4)) ChangeView(CameraMode.Isometric);
    }

    public void ChangeView(CameraMode mode)
    {
        currentMode = mode;
        
        switch (mode)
        {
            case CameraMode.FirstPerson:
                currentFollow = fpTarget;
                if (playerMesh != null) playerMesh.SetActive(false);
                break;

            case CameraMode.ThirdPerson:
                currentFollow = tpTarget;
                if (playerMesh != null) playerMesh.SetActive(true);
                break;

            case CameraMode.TwoPointFiveD:
                currentFollow = twoDTarget != null ? twoDTarget : tpTarget;
                if (playerMesh != null) playerMesh.SetActive(true);
                // Ángulo lateral fijo (90 grados)
                angle = new Vector2(90 * Mathf.Deg2Rad, 5 * Mathf.Deg2Rad); 
                break;

            case CameraMode.Isometric:
                currentFollow = isoTarget != null ? isoTarget : tpTarget;
                if (playerMesh != null) playerMesh.SetActive(true);
                // Ángulo isométrico clásico (45 grados horizontal, 30 vertical)
                angle = new Vector2(45 * Mathf.Deg2Rad, -60 * Mathf.Deg2Rad);
                break;
        }
    }

void ManejarInputsMouse()
    {
        Vector2 lookInput = Vector2.zero;

        // 1. Capturamos el input del Gamepad (Stick Derecho)
        if (Gamepad.current != null)
        {
            // Leemos el valor del stick y aplicamos sensibilidad. 
            // Multiplicamos por un factor (ej. 20f) porque el stick devuelve valores de 0 a 1, a diferencia del mouse.
            Vector2 stickInput = Gamepad.current.rightStick.ReadValue();
            lookInput.x += stickInput.x * sensitivity.x * 20f * Time.deltaTime;
            lookInput.y += stickInput.y * sensitivity.y * 20f * Time.deltaTime;
        }

        // 2. Sumamos el input del Mouse tradicional
        lookInput.x += Input.GetAxis("Mouse X") * sensitivity.x;
        lookInput.y += Input.GetAxis("Mouse Y") * sensitivity.y;

        // 3. Aplicamos la rotación solo en 1P y 3P
        if (currentMode == CameraMode.FirstPerson || currentMode == CameraMode.ThirdPerson)
        {
            // Convertimos a radianes para los cálculos de LateUpdate
            angle.x += lookInput.x * Mathf.Deg2Rad;
            angle.y += lookInput.y * Mathf.Deg2Rad;
            
            // Limitamos la rotación vertical para no dar la vuelta completa
            angle.y = Mathf.Clamp(angle.y, -80 * Mathf.Deg2Rad, 80 * Mathf.Deg2Rad);
        }

        // Zoom (habilitado en todos menos en 1ra persona)
        if (currentMode != CameraMode.FirstPerson)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            
            // También podemos habilitar zoom con los gatillos o botones del Gamepad si quisieras
            if (scroll != 0)
            {
                newDistance -= scroll * Time.deltaTime * zoomVelocity;
                newDistance = Mathf.Clamp(newDistance, minDistance, maxDistace);
            }
            defaultDistance = Mathf.Lerp(defaultDistance, newDistance, zoomSmooth);
        }
        else
        {
            defaultDistance = 0.05f; // Pegado al target en FP
        }
    }

    void LateUpdate()
    {
        if (currentFollow == null) return;

        // Cálculo de posición orbital basado en los ángulos
        Vector3 direction = new Vector3(
            Mathf.Cos(angle.x) * Mathf.Cos(angle.y),
            -Mathf.Sin(angle.y),
            -Mathf.Sin(angle.x) * Mathf.Cos(angle.y)
        );

        float distance = defaultDistance;
        Vector3[] points = GetCameraCollisionPoints(direction);

        // Verificación de colisiones para evitar atravesar paredes
        foreach (Vector3 point in points)
        {
            if (Physics.Raycast(point, direction, out RaycastHit hit, defaultDistance, obstacleLayers))
            {
                distance = Mathf.Min((hit.point - currentFollow.position).magnitude, distance);
            }
        }

        transform.position = currentFollow.position + direction * distance;
        transform.rotation = Quaternion.LookRotation(currentFollow.position - transform.position);
    }

    private void CalculateNearPlaneSize()
    {
        float height = Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad / 2) * camera.nearClipPlane;
        float width = height * camera.aspect;
        nearPlaneSize = new Vector2(width, height);
    }

    private Vector3[] GetCameraCollisionPoints(Vector3 direction)
    {
        Vector3 center = currentFollow.position + direction * (camera.nearClipPlane + 0.2f);
        Vector3 right = transform.right * nearPlaneSize.x;
        Vector3 up = transform.up * nearPlaneSize.y;

        return new Vector3[] {
            center - right + up,
            center + right + up,
            center - right - up,
            center + right - up
        };
    }
}