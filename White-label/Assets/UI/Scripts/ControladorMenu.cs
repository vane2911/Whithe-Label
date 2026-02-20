using UnityEngine;
using UnityEngine.SceneManagement; 

public class ControladorMenu : MonoBehaviour
{
    public void IniciarSimulacion()
    {
        SceneManager.LoadScene("Escena1"); 
    }

    public void Salir()
    {
        Application.Quit();
        Debug.Log("El usuario ha salido del programa");
    }

    public void RegresarAlMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuPrincipal");
    }
}