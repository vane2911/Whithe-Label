using UnityEngine;

public class CostalPendulo : MonoBehaviour
{
    [Header("Dinámica del Péndulo")]
    public float velocidadBalanceo = 2f;
    public float anguloMaximo = 35f;
    [Tooltip("Eje para el balanceo (De lado a lado)")]
    public Vector3 ejeDeBalanceo = new Vector3(0, 0, 1); 

    [Header("Dinámica del Impacto")]
    [Tooltip("Eje para el empujón (De frente hacia atrás)")]
    public Vector3 ejeDeEmpuje = new Vector3(1, 0, 0); 

    [Header("Sistema de Precisión")]
    public float margenDeCentro = 5f;
    public bool estaEnElCentro { get; private set; }

    public bool estaCongelado = false;
    private float impulsoHaciaAtras = 0f;
    
    private float tiempoOscilacion = 0f;
    private Quaternion rotacionInicial;

    void Start()
    {
        rotacionInicial = transform.localRotation;
    }

    void Update()
    {
        // 1. Balanceo normal
        if (!estaCongelado)
        {
            tiempoOscilacion += Time.deltaTime;
        }

        float anguloBase = anguloMaximo * Mathf.Sin(tiempoOscilacion * velocidadBalanceo);
        estaEnElCentro = Mathf.Abs(anguloBase) <= margenDeCentro;

        // 2. El empujón hacia atrás (Se frena suavemente)
        impulsoHaciaAtras = Mathf.Lerp(impulsoHaciaAtras, 0f, Time.deltaTime * 4f);

        // 3. Aplicar rotaciones separadas: Primero se mece, luego se empuja
        Quaternion rotacionBalanceo = Quaternion.AngleAxis(anguloBase, ejeDeBalanceo);
        Quaternion rotacionImpacto = Quaternion.AngleAxis(impulsoHaciaAtras, ejeDeEmpuje);

        transform.localRotation = rotacionInicial * rotacionBalanceo * rotacionImpacto;
    }

    public void AplicarImpacto(float fuerzaGrados)
    {
        impulsoHaciaAtras = fuerzaGrados;
    }
}