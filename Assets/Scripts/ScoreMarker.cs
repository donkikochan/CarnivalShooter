using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreMarker : MonoBehaviour
{
    public GameObject text;
    public TextMeshProUGUI scoreText;
    public GameObject imageObject;
    public TextMeshProUGUI imageText;
    public RawImage image;
    
    private float moveSpeed = 0.5f;
    private float fadeSpeed = 0.5f;
    private float lifetime = 200.0f;
    private Color markerColor;
    private Color imageColor = new Color(1, 1, 1, 1);

    private void Start()
    {
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
        
        markerColor.a -= fadeSpeed * Time.deltaTime;
        scoreText.color = markerColor;
        
        imageColor.a -= fadeSpeed * Time.deltaTime;
        imageText.color = imageColor;
        image.color = imageColor;
    }

    public void SetColor(Color color)
    {
        markerColor = color;
    }

    public void SetScore(String score)
    {
        scoreText.text = score;
    }

    public void SetImage(bool state)
    {
        text.SetActive(!state);
        imageObject.SetActive(state);
    }
}
