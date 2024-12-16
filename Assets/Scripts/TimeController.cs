using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeController : MonoBehaviour
{
    public bool beginTimer = false;
    public int time = 900;
    private float currentTime;
    public TextMeshPro timeText;
    
    // Start is called before the first frame update
    void Start()
    {
        currentTime = time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!beginTimer)
            return;
        
        if (HasEnded())
            return;
        
        SetTime((int)(currentTime -= Time.deltaTime));
    }

    public bool HasEnded()
    {
        // Convertimos a entero redondeando hacia abajo.
        int displayTime = Mathf.CeilToInt(currentTime);

        // Si el tiempo llega a 0 o menos.
        if (displayTime <= 0)
            return true;

        return false;
    }

    public void UpdateTimeText(int time)
    {
        // Convertimos el tiempo en minutos y segundos.
        int minutes = Mathf.FloorToInt(time / 60); // Minutos.
        int seconds = Mathf.FloorToInt(time % 60); // Segundos restantes.
        
        timeText.text = $"{minutes:D2}:{seconds:D2}"; // D2 asegura siempre dos dígitos
    }

    public void AddTime(int time)
    {
        this.time += time;
        UpdateTimeText(time);
    }

    public void RemoveTime(int time)
    {
        this.time -= time;
        UpdateTimeText(time);
    }

    public void ResetTime()
    {
        time = 0;
        UpdateTimeText(time);
    }

    public void SetTime(int time)
    {
        this.time = time;
        UpdateTimeText(time);
    }

    public int GetTime()
    {
        return time;
    }
}
