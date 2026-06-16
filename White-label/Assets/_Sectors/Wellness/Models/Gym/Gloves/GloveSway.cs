using UnityEngine;

public class GloveSway : MonoBehaviour
{
    [Header("Configuración de Balanceo (Sway)")]
    public float amount = 0.02f;         // Qué tanto se mueven los guantes
    public float maxAmount = 0.06f;      // Límite máximo de movimiento
    public float smoothAmount = 6f;      // Qué tan suave regresan a su lugar

    private Vector3 initialPosition;

    void Start()
    {
        // Guardamos la posición inicial local del contenedor
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        // Obtenemos el movimiento del mouse
        float movementX = -Input.GetAxis("Mouse X") * amount;
        float movementY = -Input.GetAxis("Mouse Y") * amount;

        // Limitamos el movimiento para que no se salgan de la pantalla
        movementX = Mathf.Clamp(movementX, -maxAmount, maxAmount);
        movementY = Mathf.Clamp(movementY, -maxAmount, maxAmount);

        // Calculamos la posición final deseada
        Vector3 finalPosition = new Vector3(movementX, movementY, 0) + initialPosition;

        // Suavizamos la transición usando Lerp
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition, Time.deltaTime * smoothAmount);
    }
}