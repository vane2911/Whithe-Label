using UnityEngine;
using System.IO;
using System.Collections.Generic;
using TMPro; // <--- ESTO ES NUEVO

[System.Serializable]
public class DatosJuego {
    public int puntos;
    public List<string> objetosInteractuados = new List<string>();
}

public class SaveManager : MonoBehaviour {
    public DatosJuego misDatos = new DatosJuego();
    private string rutaArchivo;
    public TextMeshProUGUI textoUI; // <--- ARRASTRA AQUÍ TU TEXTO DE PUNTOS

    void Awake() {
        // 1. Definimos la ruta del archivo como ya lo tenías
        rutaArchivo = Application.persistentDataPath + "/progreso_jugador.json";

        // 2. Si el archivo existe, lo borramos de inmediato
        if (File.Exists(rutaArchivo)) {
            File.Delete(rutaArchivo);
            Debug.Log("¡Archivo de guardado eliminado para nueva prueba!");
        }

        // 3. Reiniciamos los datos en memoria (puntos a 0 y lista vacía)
        misDatos = new DatosJuego();

        // 4. Actualizamos el texto de la pantalla (Puntos: 0)
        ActualizarTexto();
    }

    public void Guardar() {
        string json = JsonUtility.ToJson(misDatos, true);
        File.WriteAllText(rutaArchivo, json);
        Debug.Log("Puntos actuales en memoria: " + misDatos.puntos);
        //Debug.Log("Juego Guardado");
        ActualizarTexto(); // Actualiza al guardar
    }

    public void Cargar() {
        if (File.Exists(rutaArchivo)) {
            string contenido = File.ReadAllText(rutaArchivo);
            misDatos = JsonUtility.FromJson<DatosJuego>(contenido);
        }
    }

    public void ActualizarTexto() {
        if (textoUI != null) {
            textoUI.text = "Puntos: " + misDatos.puntos.ToString();
        }
    }
}