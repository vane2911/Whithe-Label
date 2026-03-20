using UnityEngine;

public class ItemProgreso : MonoBehaviour {
    [Header("Marca Blanca: ID único obligatorio")]
    public string idUnico; 

    [Header("Configuración de Recompensa")]
    [Tooltip("Cuántos puntos otorga este objeto al ser recolectado")]
    public int valorPuntos; 

    public bool desaparecerAlRecoger = true;
    public bool desactivarColliderAlRecoger = true;

    void Start() {
        // Si el ID ya está en la lista de interactuados, desactivamos la parte visual
        if (SaveManager.Instance != null && SaveManager.Instance.misDatos.objetosInteractuados.Contains(idUnico)) {
            AplicarEstadoRecogido();
        }
    }

    private void OnValidate() {
        // Solo actuamos si estamos en el Editor (no en el juego final)
        #if UNITY_EDITOR
        if (!Application.isPlaying) {
            // Si el ID está vacío O si detectamos que es un nombre genérico/repetido
            if (string.IsNullOrEmpty(idUnico)) {
                GenerarNuevoID();
            }
        }
        #endif
    }

    // Botón derecho en el componente -> Reset
    private void Reset() {
        GenerarNuevoID();
    }

    [ContextMenu("Generar Nuevo ID")] // Esto añade una opción al menú del componente
    public void GenerarNuevoID() {
        string idAleatorio = System.Guid.NewGuid().ToString().Substring(0, 4);
        
        idUnico = gameObject.name + "_" + idAleatorio;

        // Forzamos a Unity a guardar el cambio en la escena
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        #endif
    }

    public void RegistrarProgreso() 
    {
        // 1. Verificación de seguridad
        if (SaveManager.Instance == null) return;

        // 2. Verificación de duplicados (La "llave" del ID único)
        if (!SaveManager.Instance.misDatos.objetosInteractuados.Contains(idUnico)) 
        {
            SaveManager.Instance.misDatos.objetosInteractuados.Add(idUnico);
            SaveManager.Instance.misDatos.puntos += valorPuntos;
            SaveManager.Instance.Guardar();
            AplicarEstadoRecogido();

            Debug.Log($"<color=green>Éxito:</color> {idUnico} registrado. Puntos sumados: {valorPuntos}");
        }
        else 
        {
            Debug.Log($"<color=yellow>Aviso:</color> El objeto {idUnico} ya fue recolectado previamente.");
        }
    }

    private void AplicarEstadoRecogido() {        
        if (desaparecerAlRecoger) {
            gameObject.SetActive(false);
        } else {
            // Si no desaparece, quizás solo queremos que no se pueda volver a tocar
            if (desactivarColliderAlRecoger) {
                if (TryGetComponent<Collider>(out Collider c)) c.enabled = false;
            }
            // Aquí podrías cambiar el color, poner una palomita de "listo", etc.
            Debug.Log("Objeto " + idUnico + " marcado como completado, pero sigue visible.");
        }
    }
}