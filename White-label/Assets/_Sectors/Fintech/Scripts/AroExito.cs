using System.Data;
using UnityEngine;

public class AroExito : MonoBehaviour
{

    public int valorAros = 1;

    void OnTriggerEnter(Collider otro)
    {
        if (otro.CompareTag("Player")) 
        {
            GestorJuego gestor = FindFirstObjectByType<GestorJuego>();
            
            if (gestor != null)
            {
                gestor.SumarAros(valorAros);
                gestor.ReproducirSonidoExito();
            }
            Destroy(transform.parent.gameObject); 
        }
    }

}