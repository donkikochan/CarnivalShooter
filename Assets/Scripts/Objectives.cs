using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Objectives : MonoBehaviour
{
    private int points; // Puntos que otorga este objetivo al destruirse

    private ScoreController scoreController;

    private void Start()
    {
        // Encuentra el ScoreController en la escena
        scoreController = FindObjectOfType<ScoreController>();

        if (scoreController == null)
        {
            Debug.LogError("No se encontró un ScoreController en la escena.");
        }

        SetPoints(50);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile")) // Asegúrate de etiquetar los disparos como "Projectile"
        {
            scoreController.AddScore(points); // Sumar puntos
            Destroy(other.gameObject);   // Destruir el disparo
            Destroy(gameObject);         // Destruir el objetivo
        }
    }

    private void SetPoints(int points)
    {
        this.points = points;
    }
}
