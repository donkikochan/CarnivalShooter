using UnityEngine;

public class Objectives : MonoBehaviour
{
    public float 
    private int points; // Puntos que otorga este objetivo al destruirse
    private ScoreController scoreController;
    private Animator animator;
    private bool gotShoot = false;

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
        if (gotShoot)
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
        
        gotShoot = true;
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

    private void SetPoints(int points)
    {
        this.points = points;
    }
}