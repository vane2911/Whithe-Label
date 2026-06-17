using UnityEngine;

public class GirarObjeto : MonoBehaviour
{
    public float velocidadGiro = 100f;
    void Update()
    {
        transform.Rotate(Vector3.forward * velocidadGiro * Time.deltaTime);
    }
}