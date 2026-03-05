using UnityEngine;

public class DetectorDeBotes : MonoBehaviour {
    public float distancia = 3f;
    public GameObject textoPresionaE; // Arrastra aquí tu "Presiona E"
    public Transform camara; // Arrastra aquí tu Main Camera

    void Update() {
    // Esto obliga al script a usar SIEMPRE la cámara que está viendo el jugador
    Camera cam = Camera.main; 
    if (cam == null) return;

    Transform camTransform = cam.transform;
    Debug.DrawRay(camTransform.position, camTransform.forward * distancia, Color.red);

    RaycastHit hit;
    if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, distancia)) {
        // ESTO TIENE QUE SALIR EN LA CONSOLA SÍ O SÍ
        Debug.Log("RAYO TOCANDO A: " + hit.collider.name);

        ItemProgreso item = hit.collider.GetComponent<ItemProgreso>();
        if (item != null) {
            textoPresionaE.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Fire1")) {
                item.RegistrarProgreso();
            }
        } else {
            textoPresionaE.SetActive(false);
        }
    } else {
        textoPresionaE.SetActive(false);
    }
    }
}