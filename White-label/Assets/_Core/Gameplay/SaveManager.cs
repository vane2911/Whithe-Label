using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
 
[System.Serializable]
public class DatosJuego
{
    public int puntos = 0;
    public List<string> objetosInteractuados = new List<string>();
}
 
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
 
    public DatosJuego misDatos = new DatosJuego();
    private string rutaArchivo;
 
    public System.Action<DatosJuego> OnDatosActualizados;
 
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
 
        rutaArchivo = Application.persistentDataPath + "/progreso_jugador.json";
        Cargar();
    }
 
    public void Guardar()
    {
        Debug.Log("<color=cyan>[SAVE] Iniciando guardado de datos...</color>");
 
        string json = JsonUtility.ToJson(misDatos, true);
        File.WriteAllText(rutaArchivo, json);
 
        OnDatosActualizados?.Invoke(misDatos);
 
        Debug.Log("<color=green>[SAVE] Guardado completado con éxito.</color>");
    }
 
    public void Cargar()
    {
        if (File.Exists(rutaArchivo))
        {
            string contenido = File.ReadAllText(rutaArchivo);
            misDatos = JsonUtility.FromJson<DatosJuego>(contenido);
            Debug.Log("<color=yellow>[SAVE] Progreso cargado: " + misDatos.puntos + " puntos.</color>");
        }
        else
        {
            misDatos = new DatosJuego();
            Debug.Log("<color=orange>[SAVE] No se encontró archivo, datos reiniciados.</color>");
        }
 
        OnDatosActualizados?.Invoke(misDatos);
    }
 
    public void ReiniciarProgresoTotal()
    {
        if (File.Exists(rutaArchivo))
        {
            File.Delete(rutaArchivo);
            Debug.Log("<color=red>Archivo eliminado.</color>");
        }
        misDatos = new DatosJuego();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}