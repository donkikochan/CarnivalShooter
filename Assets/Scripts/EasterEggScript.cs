using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasterEggScript : MonoBehaviour
{
    public Transform player; // Arrastra aquí al jugador en el inspector
    public AudioSource audioSource;
    public float maxDistance = 20f; // Distancia máxima
    public float minDistance = 2f; // Distancia mínima

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        // Calcula el volumen basado en la distancia
        if (distance <= minDistance)
        {
            audioSource.volume = 1f; // Volumen máximo
        }
        else if (distance >= maxDistance)
        {
            audioSource.volume = 0f; // Silencio total
        }
        else
        {
            // Ajuste lineal del volumen
            float t = (distance - minDistance) / (maxDistance - minDistance);
            audioSource.volume = 1f - t;
        }
    }
}
