using UnityEngine;

public class Objectives : MonoBehaviour
{
    private int points; // Puntos que otorga este objetivo al destruirse
    private ScoreController scoreController;
    private Animator animator;

    private void Start()
    {
        // Encuentra el ScoreController en la escena
        scoreController = FindObjectOfType<ScoreController>();
        animator = GetComponent<Animator>();

        if (scoreController == null)
        {
            Debug.LogError("No se encontró un ScoreController en la escena.");
        }

        SetPoints(50);
    }

    public void OnShot()
    {
        // Sumar puntos al ScoreController
        scoreController.AddScore(points);

        // Activar la animación
        animator.SetTrigger("close");

        // Obtener la duración del clip actual de animación
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0)
        {
            float animationTime = clipInfo[0].clip.length;

            // Destruir el objeto después de la animación
            Invoke(nameof(DestroyAfterAnimation), animationTime);
        }
        else
        {
            Debug.LogWarning("No se encontró ningún clip de animación.");
            DestroyAfterAnimation(); // Si no hay animación, destruir directamente
        }
    }

    private void DestroyAfterAnimation()
    {
        Destroy(gameObject);
    }

    private void SetPoints(int points)
    {
        this.points = points;
    }
}