using UnityEngine;
using System.Collections;

public class PunchingBagController : MonoBehaviour
{
    [Header("Referencias Físicas")]
    [SerializeField] private CostalPendulo pendulo;
    [SerializeField] private Renderer bagRenderer;

    [Header("Efectos Visuales")]
    [SerializeField] private ParticleSystem hitParticles;
    [SerializeField] private ParticleSystem superHitParticles;
    [SerializeField] private ParticleSystem destroyParticles;

    private Material bagMaterial;
    
    // IMPORTANTE: Este nombre debe coincidir con el campo "Reference" en tu Shader Graph
    private readonly string variableShaderGrietas = "_IntensidadGrietas";

    // Puente para que el GameManager lea el centro sin tocar al péndulo directamente
    public bool IsInCenterZone => pendulo != null && pendulo.estaEnElCentro;

    private void Awake()
    {
        if (bagRenderer != null)
            bagMaterial = bagRenderer.material; // Crea una copia única del material
    }

    public void ResetBag()
    {
        gameObject.SetActive(true);
        if (pendulo != null) pendulo.estaCongelado = false;
        
        // Reiniciar el Shader a cero daños
        if (bagMaterial != null) bagMaterial.SetFloat(variableShaderGrietas, 0f);
    }

    public void OnHoldStart()
    {
        // Congelar el balanceo en el aire
        if (pendulo != null) pendulo.estaCongelado = true;
    }

    public void OnPunchLanded(int punchesLanded, int totalPunches, bool isSuperPunch)
    {
        // Soltar el balanceo
        if (pendulo != null) pendulo.estaCongelado = false;
        pendulo.AplicarImpacto(30f);

        // --- LA MAGIA DEL SHADER GRAPH ---
        // Calcula el porcentaje exacto (Ej: 3 golpes de 10 = 0.3) y lo manda al material
        float nivelDeDestruccion = (float)punchesLanded / totalPunches;
        if (bagMaterial != null) bagMaterial.SetFloat(variableShaderGrietas, nivelDeDestruccion);

        // Partículas
        if (isSuperPunch && superHitParticles != null) superHitParticles.Play();
        else if (!isSuperPunch && hitParticles != null) hitParticles.Play();

        if (punchesLanded >= totalPunches)
            StartCoroutine(DestroyBagSequence());
    }

    public void OnInvalidRelease()
    {
        // Soltar el balanceo porque el golpe falló
        if (pendulo != null) pendulo.estaCongelado = false;
    }

    private IEnumerator DestroyBagSequence()
    {
        if (destroyParticles != null) destroyParticles.Play();
        
        // Esperamos medio segundo para que el jugador vea el nivel 1.0 del Shader (Agujeros máximos)
        yield return new WaitForSeconds(0.5f);
        
        // Desactivamos la lona para revelar el producto interior
        gameObject.SetActive(false);
    }
}