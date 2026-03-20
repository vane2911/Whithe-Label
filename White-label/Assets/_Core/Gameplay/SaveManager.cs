using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class DatosJuego {
    public int puntos = 0;
    public List<string> objetosInteractuados = new List<string>();
}

public class SaveManager : MonoBehaviour {
    public static SaveManager Instance; 

    public DatosJuego misDatos = new DatosJuego();
    private string rutaArchivo;
    public TextMeshProUGUI textoUI;

    void Awake() {
        // Configuración Singleton
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
            return;
        }

        rutaArchivo = Application.persistentDataPath + "/progreso_jugador.json";
        
        // OPCIONAL: Para tus pruebas, borramos el progreso al iniciar
        // Si quieres que el progreso sea real, comenta la línea de abajo
        //if (File.Exists(rutaArchivo)) File.Delete(rutaArchivo);

        Cargar();
        ActualizarTexto();
    }

    public void Guardar() {
    // Evento: Inicio del guardado (Start)
        Debug.Log("<color=cyan>[SAVE] Iniciando guardado de datos...</color>");
        
        string json = JsonUtility.ToJson(misDatos, true);
        File.WriteAllText(rutaArchivo, json);
        
        ActualizarTexto();
        
        // Evento: Guardado completado (Complete)
        Debug.Log("<color=green>[SAVE] Guardado completado con éxito.</color>");
    }

public void Cargar() {
    if (File.Exists(rutaArchivo)) {
        string contenido = File.ReadAllText(rutaArchivo);
        misDatos = JsonUtility.FromJson<DatosJuego>(contenido);
        Debug.Log("<color=yellow>[SAVE] Progreso cargado: " + misDatos.puntos + " puntos.</color>");
    } else {
        // Aseguramos que los datos estén limpios si no hay archivo
        misDatos = new DatosJuego(); 
        Debug.Log("<color=orange>[SAVE] No se encontró archivo, datos reiniciados.</color>");
    }
}

    public void ActualizarTexto() {
        if (textoUI != null) {
            textoUI.text = "Puntos: " + misDatos.puntos.ToString();
        }
    }

    public void ReiniciarProgresoTotal() {
        if (File.Exists(rutaArchivo)) {
            File.Delete(rutaArchivo);
            Debug.Log("<color=red>Archivo eliminado.</color>");
        }
        misDatos = new DatosJuego();
        // Recarga de escena limpia
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= AlCargarEscena;
    }

    void AlCargarEscena(Scene scene, LoadSceneMode mode) {
        // Buscamos el nuevo componente de texto en la escena recargada
        textoUI = GameObject.Find("TextoPuntos")?.GetComponent<TextMeshProUGUI>();
        ActualizarTexto();
    }

}