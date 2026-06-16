using UnityEngine;

/// <summary>
/// ScriptableObject de configuración White Label para Brand Impact.
/// Cada marca tiene su propio asset con sus colores, logo y premio.
/// Crear desde: Assets > Create > WhiteLabel > BrandImpactConfig
/// </summary>
[CreateAssetMenu(fileName = "BrandConfig_NombreMarca", menuName = "WhiteLabel/BrandImpactConfig")]
public class BrandImpactConfig : ScriptableObject
{
    [Header("Identidad de marca")]
    public string brandName = "Marca";
    public Color bagColor = Color.white;
    public Color gloveColor = Color.white;
    public Sprite brandLogo;           // Logo que aparece en el costal

    [Header("Premio final")]
    public Sprite prizeSprite;         // Imagen del premio a revelar
    public string prizeName = "Premio";
    public string prizeDescription = "Descripción del premio";

    [Header("Visual - Entorno")]
    public Material environmentMaterial;   // Material/skybox del entorno (opcional)
    public Color accentColor = Color.cyan; // Color de efectos UI (brillo, bordes)

    [Header("Audio (opcional)")]
    public AudioClip brandJingle;      // Sonido al ganar
}