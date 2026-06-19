using UnityEngine;

public class ControlCaida : MonoBehaviour
{
    [Header("Movimiento Base")]
    public float velocidadLateral = 20f; 
    public float velocidadVertical = 15f; 
    public float velocidadAvance = 15f; 
    public float impulsoSalto = 5f;    
    public float gravedad = 15f;       
    public float metrosDeCaida = 8f;   
    public float velocidadCaminar = 5f;

    [Header("Interacción con Aros")]
    public int vidas = 3;
    public float aumentoVelocidad = 5f;

    [Header("Referencias")]
    public VueloAvion scriptDelAvion; 
    public GestorJuego gestorCentral;

    private bool estaVolando = false; 
    private float velocidadY = 0f; 
    private float alturaEstable;    

    void Update()
    {
        if (Time.timeScale == 0f) return;
        
        // 1. Detectar el inicio del salto MANUAL
        if (!estaVolando && Input.GetKeyDown(KeyCode.Space))
        {
            estaVolando = true; 
            velocidadY = impulsoSalto; 
            alturaEstable = transform.position.y - metrosDeCaida; 
            
            transform.SetParent(null); 

            if (scriptDelAvion != null)
            {
                // --- ¡ELIMINACIÓN FÍSICA! ---
                // Le arrancamos la física al avión para que sea solo un holograma visual
                Destroy(scriptDelAvion.GetComponent<Collider>());
                Destroy(scriptDelAvion.GetComponent<Rigidbody>());

                Destroy(scriptDelAvion.gameObject, 20f); 
            }
        }

        // 2. Comportamiento para caminar ANTES de saltar (Caída automática)
        if (!estaVolando)
        {
            float caminarX = Input.GetAxis("Horizontal");
            float caminarZ = Input.GetAxis("Vertical");

            Vector3 direccionCaminar = new Vector3(caminarX, 0, caminarZ);
            transform.Translate(direccionCaminar * velocidadCaminar * Time.deltaTime, Space.Self);

            Debug.DrawRay(transform.position + Vector3.up * 1f, Vector3.down * 5f, Color.red);

            if (!Physics.Raycast(transform.position + Vector3.up * 1f, Vector3.down, 5f))
            {
                estaVolando = true; 
                velocidadY = 0f; 
                alturaEstable = transform.position.y - metrosDeCaida; 
                
                transform.SetParent(null); 

                if (scriptDelAvion != null)
                {
                    // --- ¡ELIMINACIÓN FÍSICA! ---
                    Destroy(scriptDelAvion.GetComponent<Collider>());
                    Destroy(scriptDelAvion.GetComponent<Rigidbody>());

                    Destroy(scriptDelAvion.gameObject, 20f); 
                }
            }
        }

        // 3. Comportamiento en el aire con vuelo libre
        if (estaVolando)
        {
            float movimientoX = Input.GetAxis("Horizontal");
            float movimientoY = Input.GetAxis("Vertical"); 

            alturaEstable += movimientoY * velocidadVertical * Time.deltaTime;

            if (alturaEstable < 2f) alturaEstable = 2f; 

            if (transform.position.y > alturaEstable + 2f)
            {
                velocidadY -= gravedad * Time.deltaTime;
            }
            else 
            {
                velocidadY = Mathf.Lerp(velocidadY, 0f, Time.deltaTime * 4f);
                float alturaSuavizada = Mathf.Lerp(transform.position.y, alturaEstable, Time.deltaTime * 5f);
                transform.position = new Vector3(transform.position.x, alturaSuavizada, transform.position.z);
            }

            Vector3 direccionVuelo = new Vector3(movimientoX * velocidadLateral, velocidadY, velocidadAvance);
            transform.Translate(direccionVuelo * Time.deltaTime, Space.World);

            Quaternion poseDeVuelo = Quaternion.Euler(75, transform.rotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, poseDeVuelo, Time.deltaTime * 3f);

            if (velocidadAvance >= 60f)
            {
                if (gestorCentral != null && gestorCentral.vidas > 0)
                {
                    gestorCentral.GanarJuego();
                }
            }
        }
    }

    // --- SECCIÓN DE COLISIONES ---
    void OnTriggerEnter(Collider otro)
    {
        if (otro.CompareTag("AroExito"))
        {
            velocidadAvance += aumentoVelocidad; 
            Destroy(otro.transform.parent.gameObject); 
        }
        else if (otro.CompareTag("AroFallo"))
        {
            if (gestorCentral != null) gestorCentral.PerderVida();
            Destroy(otro.transform.parent.gameObject);
        }
    }
}