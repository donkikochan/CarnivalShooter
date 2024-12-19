using TMPro;
using UnityEngine;

public class ScoreMarker : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    private float moveSpeed = 0.5f;
    private float fadeSpeed = 0.5f;
    private float lifetime = 200.0f;

    private Color originalColor;

    private void Start()
    {
        originalColor = scoreText.color;
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null && Camera.main != null)
        {
            canvas.worldCamera = Camera.main;
        }
        else
        {
            Debug.LogWarning("El marcador no tiene un Canvas o no se encontró la cámara principal.");
        }
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
        
        Color color = scoreText.color;
        color.a -= fadeSpeed * Time.deltaTime;
        scoreText.color = color;
    }

    public void SetScore(float score)
    {
        scoreText.text = score.ToString();
    }
}
