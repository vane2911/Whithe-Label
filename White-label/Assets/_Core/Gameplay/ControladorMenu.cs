using UnityEngine;
using UnityEngine.SceneManagement;
 
public class ControladorMenu : MonoBehaviour
{
    [Header("WhiteLabel - Nombres de Escena")]
    [Tooltip("Nombre exacto de la escena principal del juego (debe estar en Build Settings)")]
    [SerializeField] private string nombreEscenaJuego;
 
    [Tooltip("Nombre exacto de la escena del menú principal")]
    [SerializeField] private string nombreEscenaMenu;
 
    public void IniciarSimulacion()
    {
        SceneManager.LoadScene(nombreEscenaJuego);
    }
 
    public void Salir()
    {
        Application.Quit();
        Debug.Log("El usuario ha salido del programa");
    }
 
    public void RegresarAlMenu()
    {
        SceneManager.LoadScene(nombreEscenaMenu);
    }
}