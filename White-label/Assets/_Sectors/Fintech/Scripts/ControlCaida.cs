using UnityEngine;

public class ControlCaida : MonoBehaviour
{
    public float velocidadLateral = 20f; 
    public float velocidadAvance = 15f; 

    public float impulsoSalto = 5f;    
    public float gravedad = 15f;       
    public float metrosDeCaida = 8f;   

    public VueloAvion scriptDelAvion; 

    public float velocidadCaminar = 5f;

    private bool estaVolando = false; 
    private float velocidadY = 0f; 
    private float alturaEstable;       

    void Update()
    {
        // 1. Detectar el inicio del salto MANUAL (presionando Espacio)
        if (!estaVolando && Input.GetKeyDown(KeyCode.Space))
        {
            estaVolando = true; 
            velocidadY = impulsoSalto; 
            alturaEstable = transform.position.y - metrosDeCaida; 
            
            // Nos soltamos del avión
            transform.SetParent(null); 

            if (scriptDelAvion != null)
            {
                Destroy(scriptDelAvion.gameObject, 20f); 
            }
        }

        // 2. Comportamiento para caminar ANTES de saltar
        if (!estaVolando)
        {
            float caminarX = Input.GetAxis("Horizontal");
            float caminarZ = Input.GetAxis("Vertical");

            Vector3 direccionCaminar = new Vector3(caminarX, 0, caminarZ);
            transform.Translate(direccionCaminar * velocidadCaminar * Time.deltaTime, Space.Self);

            // DIBUJAMOS EL LÁSER: Para que lo puedas ver de color rojo en tu pestaña "Scene"
            Debug.DrawRay(transform.position + Vector3.up * 1f, Vector3.down * 5f, Color.red);

            // ¡EL LÁSER CORREGIDO!: Empezamos 1 metro más arriba y disparamos 5 metros hacia abajo.
            if (!Physics.Raycast(transform.position + Vector3.up * 1f, Vector3.down, 5f))
            {
                estaVolando = true; 
                velocidadY = 0f; 
                alturaEstable = transform.position.y - metrosDeCaida; 
                
                transform.SetParent(null); 

                if (scriptDelAvion != null)
                {
                    Destroy(scriptDelAvion.gameObject, 10f); 
                }
            }
        }

        // 3. Comportamiento en el aire con frenado suave
        if (estaVolando)
        {
            float movimientoX = Input.GetAxis("Horizontal");

            // Si estamos más arriba de la "zona de frenado", caemos normal
            if (transform.position.y > alturaEstable + 2f)
            {
                velocidadY -= gravedad * Time.deltaTime;
            }
            else 
            {
                // Colchón de aire para un aterrizaje natural
                velocidadY = Mathf.Lerp(velocidadY, 0f, Time.deltaTime * 4f);
                float alturaSuavizada = Mathf.Lerp(transform.position.y, alturaEstable, Time.deltaTime * 3f);
                transform.position = new Vector3(transform.position.x, alturaSuavizada, transform.position.z);
            }

            // Aplicamos el movimiento
            Vector3 direccionVuelo = new Vector3(movimientoX * velocidadLateral, velocidadY, velocidadAvance);
            transform.Translate(direccionVuelo * Time.deltaTime, Space.World);

            // Inclinación del cuerpo
            Quaternion poseDeVuelo = Quaternion.Euler(75, transform.rotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, poseDeVuelo, Time.deltaTime * 3f);
        }
    }
}