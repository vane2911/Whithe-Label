using UnityEngine;

public class ItemProgreso : MonoBehaviour {
    public string idUnico; // Nombre único para este objeto
    private SaveManager sm;

    void Start() {
        sm = Object.FindAnyObjectByType<SaveManager>();
        // Si ya está guardado, desaparece al empezar
        if (sm != null && sm.misDatos.objetosInteractuados.Contains(idUnico)) {
            gameObject.SetActive(false);
        }
    }

    public void RegistrarProgreso() {
        if (sm != null && !sm.misDatos.objetosInteractuados.Contains(idUnico)) {
            sm.misDatos.objetosInteractuados.Add(idUnico);
            sm.misDatos.puntos += 10; // Sumar puntos/monedas
            sm.Guardar(); // Guardar automáticamente
            gameObject.SetActive(false); // Desaparecer al recoger
        }
    }
}