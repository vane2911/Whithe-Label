using UnityEngine;
using TMPro; // Necesario para TextMeshPro

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance; // Singleton para llamarlo fácilmente
    public GameObject promptText;

    void Awake()
    {
        Instance = this;
    }

    public void ShowPrompt(bool state)
    {
        if (promptText != null)
            promptText.SetActive(state);
    }
}