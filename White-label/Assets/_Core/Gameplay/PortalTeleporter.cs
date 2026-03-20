using UnityEngine;
using UnityEngine.SceneManagement; 

public class PortalTeleporter : MonoBehaviour
{
    [SerializeField] private string sceneToLoad; 

    private void OnTriggerEnter(Collider other)
    {
        // Verifica si quien entró al portal es el jugador
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}