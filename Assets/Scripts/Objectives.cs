using System;
using Autohand;
using Unity.VisualScripting;
using UnityEngine;

public class Objectives : MonoBehaviour
{
    public event Action OnDestroyed;
    public float duration = 5.0f; // Tiempo antes de que el objeto se destruya automáticamente
    public int points = 50; // Puntos que otorga este objetivo al destruirse
    public int timeThatGives = 10;
    public bool rechargesAmmo = false;
    public bool givesTime = false;
    public bool givesNegativePoints = false;
    public bool givesNegativeTime = false;
    public bool movable = false;
    public GameObject scoreMarkerPrefab;
    public float markerHeightOffset = 2f;
    
    private ScoreController scoreController;
    private TimeController timeController;
    private AudioController AudioController;

    private Animator animator;
    private bool gotShot = false;

    private void OnDestroy()
    {
        OnDestroyed?.Invoke();
    }

    private void Start()
    {
        // Encuentra los Controllers en la escena
        scoreController = FindObjectOfType<ScoreController>();
        timeController = FindObjectOfType<TimeController>();
        AudioController = FindObjectOfType<AudioController>();
        animator = GetComponent<Animator>();

        if (scoreController == null)
        {
            Debug.LogError("No se encontró un ScoreController en la escena.");
        }
        
        if (movable)
        {
            animator.SetBool("move", true);
        }

        // Inicia la destrucción automática después de 5 segundos
        Invoke(nameof(DestroyAutomatically), duration);
    }

    public void OnShot()
    {
        if (gotShot)
            return;

        if (givesNegativePoints)
        {
            scoreController.RemoveScore(points);
            AudioController.PlayImpactSound(ImpactSounds.NEGATIVE_HIT);

        }
        else
        {
            // Sumar puntos al ScoreController
            scoreController.AddScore(points);
            AudioController.PlayImpactSound(ImpactSounds.POSITIVE_HIT);
            AudioController.PlayImpactSound(ImpactSounds.DUCK_SOUND);


        }

        if (rechargesAmmo)
        {
            GameObject de = GameObject.FindGameObjectWithTag("WeaponDE");
            GameObject svd = GameObject.FindGameObjectWithTag("WeaponSVD");
            GameObject spas = GameObject.FindGameObjectWithTag("WeaponSPAS");
            
            if (de)
                de.GetComponent<AutoGun>().SetAmmo(8);
            
            if (svd)
                svd.GetComponent<AutoGun>().SetAmmo(11);
            
            if (spas)
                spas.GetComponent<AutoGun>().SetAmmo(17);
            
            AudioController.PlayImpactSound(ImpactSounds.RELOAD_SOUND);
        }

        if (givesTime)
        {
            if (givesNegativeTime)
            {
                timeController.RemoveTime(timeThatGives);
            }
            else
            {
                timeController.AddTime(timeThatGives);
            }
        }
        
        // Instanciar marcador de daño
        GameObject marker = Instantiate(scoreMarkerPrefab, transform.position, transform.rotation);
        
        if (Camera.main != null)
        {
            // Orientar el marcador hacia la cámara
            marker.transform.LookAt(Camera.main.transform);

            // Invertir para que el texto sea legible
            marker.transform.Rotate(0, 180, 0);
        }

        // Establecer el daño en el marcador
        ScoreMarker scoreMarker = marker.GetComponent<ScoreMarker>();
        String symbol = givesNegativePoints ? "-" : "+";
        Color color = givesNegativePoints && !rechargesAmmo
            ? new Color(255f / 255f, 40f / 255f, 40f / 255f, 255f / 255f) 
            : new Color(100f / 255f, 255f / 255f, 70f / 255f, 255f / 255f);

        if (scoreMarker != null)
        {
            scoreMarker.SetScore(symbol + points);
            scoreMarker.SetColor(color);
            scoreMarker.SetImage(rechargesAmmo);
        }

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
}
