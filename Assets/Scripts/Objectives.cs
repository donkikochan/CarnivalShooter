using UnityEngine;

public class Objectives : MonoBehaviour
{
    public float duration = 5.0f; // Tiempo antes de que el objeto se destruya automáticamente
    private int points; // Puntos que otorga este objetivo al destruirse
    private ScoreController scoreController;
    private Animator animator;
    private bool gotShot = false;

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

        // Inicia la destrucción automática después de 5 segundos
        Invoke(nameof(DestroyAutomatically), duration);
    }

    public void OnShot()
    {
        if (gotShot)
            return;

        // Sumar puntos al ScoreController
        scoreController.AddScore(points);

        // Activar la animación de cierre
        animator.SetTrigger("close");

        // Obtener la duración de la animación `close`
        float closeAnimationTime = GetAnimationClipLength("close");

        if (closeAnimationTime > 0)
        {
            // Destruir el objeto después de la animación
            Invoke(nameof(DestroyAfterAnimation), closeAnimationTime);
        }
        else
        {
            Debug.LogWarning("No se encontró el clip de animación 'close'.");
            DestroyAfterAnimation(); // Destruir directamente si no se encuentra el clip
        }

        gotShot = true;
    }

    private float GetAnimationClipLength(string clipName)
    {
        // Buscar en todos los clips del Animator
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }

        return 0f; // Retorna 0 si no se encuentra el clip
    }

    private void DestroyAfterAnimation()
    {
        Destroy(gameObject);
    }

    private void DestroyAutomatically()
    {
        if (!gotShot)
        {
            // Si no fue destruido por un disparo, activa la animación de cierre antes de destruir
            animator.SetTrigger("close");

            float closeAnimationTime = GetAnimationClipLength("close");

            if (closeAnimationTime > 0)
            {
                Invoke(nameof(DestroyAfterAnimation), closeAnimationTime);
            }
            else
            {
                DestroyAfterAnimation(); // Si no hay animación, destrúyelo directamente
            }
        }
    }

    private void SetPoints(int points)
    {
        this.points = points;
    }
}
