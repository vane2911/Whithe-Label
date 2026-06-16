using UnityEngine;
using System.Collections;

public class GloveController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform guanteIzquierdo;
    [SerializeField] private Transform guanteDerecho;

    [Header("Configuración del Golpe")]
    [SerializeField] private float distanciaMaximaGolpe = 1.5f; 
    // Ahora tenemos márgenes separados por si un guante es más largo que el otro
    [SerializeField] private float margenImpactoIzq = 0.4f; 
    [SerializeField] private float margenImpactoDer = 0.45f; 
    [SerializeField] private float convergenciaCentro = 0.3f; 
    
    [Header("Animación Avanzada (Curvas)")]
    [SerializeField] private AnimationCurve curvaIda = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private AnimationCurve curvaRegreso = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private float rotacionGolpe = 45f;

    [Header("Efecto de Reposo (Respiración)")]
    [SerializeField] private float intensidadRespiracion = 0.015f;
    [SerializeField] private float velocidadRespiracion = 2f;

    private Vector3 posInicialIzq;
    private Vector3 posInicialDer;
    private Vector3 offsetGolpeIzq = Vector3.zero;
    private Vector3 offsetGolpeDer = Vector3.zero;

    private Quaternion rotInicialIzq;
    private Quaternion rotInicialDer;
    private Quaternion rotExtraIzq = Quaternion.identity;
    private Quaternion rotExtraDer = Quaternion.identity;

    private bool estaGolpeandoIzq = false;
    private bool estaGolpeandoDer = false;
    private bool estaCargando = false;

    private void Start()
    {
        if (guanteIzquierdo != null) 
        {
            posInicialIzq = guanteIzquierdo.localPosition;
            rotInicialIzq = guanteIzquierdo.localRotation;
        }
        if (guanteDerecho != null) 
        {
            posInicialDer = guanteDerecho.localPosition;
            rotInicialDer = guanteDerecho.localRotation;
        }
    }

    private void Update()
    {
        float offsetY = Mathf.Sin(Time.time * velocidadRespiracion) * intensidadRespiracion;
        Vector3 respiracion = new Vector3(0, offsetY, 0);

        if (guanteIzquierdo != null)
        {
            guanteIzquierdo.localPosition = posInicialIzq + respiracion + offsetGolpeIzq;
            guanteIzquierdo.localRotation = rotInicialIzq * rotExtraIzq;
        }

        if (guanteDerecho != null)
        {
            guanteDerecho.localPosition = posInicialDer + respiracion + offsetGolpeDer;
            guanteDerecho.localRotation = rotInicialDer * rotExtraDer;
        }
    }

    public void LanzarGolpeIzquierdo()
    {
        if (!estaGolpeandoIzq && guanteIzquierdo != null)
            StartCoroutine(AnimarGolpe(true));
    }

    public void LanzarGolpeDerecho()
    {
        if (!estaGolpeandoDer && guanteDerecho != null)
            StartCoroutine(AnimarGolpe(false));
    }

    private IEnumerator AnimarGolpe(bool esIzquierdo)
    {
        if (esIzquierdo) estaGolpeandoIzq = true;
        else estaGolpeandoDer = true;
        
        estaCargando = true;

        // 1. Detección dinámica de distancia
        float distanciaReal = distanciaMaximaGolpe;
        float margenActual = esIzquierdo ? margenImpactoIzq : margenImpactoDer;

        Vector3 direccionAjustada = transform.forward + (transform.up * -0.15f); 

        if (Physics.Raycast(transform.position, direccionAjustada, out RaycastHit hit, distanciaMaximaGolpe))
        {
            distanciaReal = Mathf.Max(0.1f, hit.distance - margenActual);
        }

        // 2. Cálculo de dirección (Usando la cámara como guía para no fallar al costal)
// Usamos la dirección que apunta hacia abajo para que el guante sepa a dónde ir
Vector3 puntoImpactoMundo = transform.position + (direccionAjustada.normalized * distanciaReal);        // Añadimos la convergencia manual para asegurar que busquen el centro
        puntoImpactoMundo += (esIzquierdo ? transform.right : -transform.right) * (convergenciaCentro * 0.5f);

        Transform padreGuantes = esIzquierdo ? guanteIzquierdo.parent : guanteDerecho.parent;
        Vector3 objetivoGolpe = padreGuantes.InverseTransformDirection(puntoImpactoMundo - padreGuantes.position);

        // 3. Rotación corregida para modelos espejados
        // Si el izquierdo no gira, cambia el signo de rotacionGolpe aquí
        float anguloZ = esIzquierdo ? rotacionGolpe : -rotacionGolpe;
        Quaternion rotacionObjetivo = Quaternion.Euler(0, 0, anguloZ);

        // FASE 1: IDA
        float tiempoIda = 0.08f;   
        float cronometro = 0f;
        while (cronometro < tiempoIda)
        {
            cronometro += Time.deltaTime;
            float valorCurva = curvaIda.Evaluate(cronometro / tiempoIda);
            ActualizarPosicionGuante(esIzquierdo, Vector3.zero, objetivoGolpe, valorCurva, rotacionObjetivo);
            yield return null;
        }

        // PAUSA DE CARGA
        yield return new WaitUntil(() => !estaCargando);

        // FASE 2: REGRESO
        float tiempoRegreso = 0.2f; // Un poquito más lento para que sea fluido
        cronometro = 0f;
        while (cronometro < tiempoRegreso)
        {
            cronometro += Time.deltaTime;
            float valorCurva = curvaRegreso.Evaluate(cronometro / tiempoRegreso);
            ActualizarPosicionGuante(esIzquierdo, objetivoGolpe, Vector3.zero, valorCurva, rotacionObjetivo);
            yield return null;
        }

        FinalizarGolpe(esIzquierdo);
    }

    private void ActualizarPosicionGuante(bool esIzq, Vector3 inicio, Vector3 fin, float t, Quaternion rotObj) 
    {
        if (esIzq) {
            offsetGolpeIzq = Vector3.LerpUnclamped(inicio, fin, t);
            rotExtraIzq = Quaternion.SlerpUnclamped(inicio == Vector3.zero ? Quaternion.identity : rotObj, inicio == Vector3.zero ? rotObj : Quaternion.identity, t);
        } else {
            offsetGolpeDer = Vector3.LerpUnclamped(inicio, fin, t);
            rotExtraDer = Quaternion.SlerpUnclamped(inicio == Vector3.zero ? Quaternion.identity : rotObj, inicio == Vector3.zero ? rotObj : Quaternion.identity, t);
        }
    }

    private void FinalizarGolpe(bool esIzq) 
    {
        if (esIzq) { offsetGolpeIzq = Vector3.zero; rotExtraIzq = Quaternion.identity; estaGolpeandoIzq = false; }
        else { offsetGolpeDer = Vector3.zero; rotExtraDer = Quaternion.identity; estaGolpeandoDer = false; }
    }

    public void DetenerCarga() 
    {
        estaCargando = false;
    }
}