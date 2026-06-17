using UnityEngine;

public class ControlCaida : MonoBehaviour
{
    [Header("Movimiento Base")]
    public float velocidadLateral = 20f; 
    public float velocidadVertical = 15f;  // NUEVO: Qué tan rápido sube y baja
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

            Debug.DrawRay(transform.position + Vector3.up * 1f, Vector3.down * 5f, Color.red);

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

        // 3. Comportamiento en el aire con vuelo libre
        if (estaVolando)
        {
            float movimientoX = Input.GetAxis("Horizontal");
            float movimientoY = Input.GetAxis("Vertical"); // NUEVO: Detecta teclas Arriba/Abajo o W/S

            // NUEVO: Modificamos la "altura ideal" según lo que presione el jugador
            alturaEstable += movimientoY * velocidadVertical * Time.deltaTime;

            // Límite opcional (para que no se entierre en el pasto)
            if (alturaEstable < 2f) alturaEstable = 2f; 

            if (transform.position.y > alturaEstable + 2f)
            {
                velocidadY -= gravedad * Time.deltaTime;
            }
            else 
            {
                velocidadY = Mathf.Lerp(velocidadY, 0f, Time.deltaTime * 4f);
                // Subí un poco el multiplicador final (a 5f) para que responda más rápido al control vertical
                float alturaSuavizada = Mathf.Lerp(transform.position.y, alturaEstable, Time.deltaTime * 5f);
                transform.position = new Vector3(transform.position.x, alturaSuavizada, transform.position.z);
            }

            Vector3 direccionVuelo = new Vector3(movimientoX * velocidadLateral, velocidadY, velocidadAvance);
            transform.Translate(direccionVuelo * Time.deltaTime, Space.World);

            // Inclinación del cuerpo
            Quaternion poseDeVuelo = Quaternion.Euler(75, transform.rotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, poseDeVuelo, Time.deltaTime * 3f);

            if (velocidadAvance >= 60f)
            {
                // Nos aseguramos de tener el Gestor conectado y seguir vivos
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
            
            // Le avisamos al GameManager que sume 10 puntos
            //if (gestorCentral != null) gestorCentral.SumarPuntos(10); 
            
            Destroy(otro.transform.parent.gameObject); 
        }
        else if (otro.CompareTag("AroFallo"))
        {
            // Le avisamos al GameManager que quite una vida
            if (gestorCentral != null) gestorCentral.PerderVida();
            
            Destroy(otro.transform.parent.gameObject);
        }
    }
}