using UnityEngine;

/// <summary>
/// ScriptableObject de configuración White Label para Pulse Reflex.
/// Crear desde: Assets > Create > WhiteLabel > PulseReflexConfig
/// </summary>
[CreateAssetMenu(fileName = "PulseReflexConfig_NombreMarca", menuName = "WhiteLabel/PulseReflexConfig")]
public class PulseReflexConfig : ScriptableObject
{
    [Header("Identidad de marca")]
    public string brandName = "Marca";
    public Color accentColor = Color.cyan;

    [Header("Objetos a tocar (spawneables)")]
    [Tooltip("Sprites de logos/productos que aparecen en pantalla")]
    public Sprite[] spawnableSprites;

    [Header("Premio central")]
    public Sprite prizeSprite;
    public string prizeName = "Premio";
    public string prizeDescription = "Descripción del premio";

    [Header("Entorno")]
    public Material environmentMaterial;

    [Header("Audio (opcional)")]
    public AudioClip hitSound;
    public AudioClip missSound;
    public AudioClip winJingle;
}