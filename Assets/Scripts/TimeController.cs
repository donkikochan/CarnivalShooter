using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeController : MonoBehaviour
{
    public bool beginTimer = false;
    public int time = 900; // Tiempo inicial en segundos.
    private float currentTime; // Tiempo actual, con precisión decimal.
    public TextMeshPro timeText;

    void Start()
    {
        currentTime = time; // Inicializa el tiempo actual.
        UpdateTimeText((int)currentTime); // Actualiza el texto al inicio.
    }

    void Update()
    {
        if (!beginTimer || HasEnded())
            return;

        currentTime -= Time.deltaTime; // Resta tiempo.
        UpdateTimeText(Mathf.CeilToInt(currentTime)); // Actualiza el texto.
    }

    public bool HasEnded()
    {
        if (currentTime <= 0)
        {
            currentTime = 0; // Asegura que no sea negativo.
            UpdateTimeText(0); // Muestra 0 en pantalla.
            beginTimer = false; // Detén el temporizador.
            return true;
        }
        return false;
    }

    public void UpdateTimeText(int time)
    {
        int minutes = time / 60;
        int seconds = time % 60;
        timeText.text = $"{minutes:D2}:{seconds:D2}";
    }

    public void AddTime(int seconds)
    {
        currentTime += seconds; // Agrega tiempo.
        if (currentTime > time) time = (int)currentTime; // Asegura coherencia.
        UpdateTimeText(Mathf.CeilToInt(currentTime));
    }

    public void RemoveTime(int seconds)
    {
        currentTime -= seconds; // Reduce tiempo.
        if (currentTime < 0) currentTime = 0; // Evita negativos.
        UpdateTimeText(Mathf.CeilToInt(currentTime));
    }

    public void ResetTime()
    {
        currentTime = time; // Restaura al tiempo inicial.
        UpdateTimeText((int)currentTime);
        beginTimer = false; // Detiene el temporizador.
    }

    public void SetTime(int newTime)
    {
        currentTime = newTime;
        time = newTime; // Asegura coherencia.
        UpdateTimeText(Mathf.CeilToInt(currentTime));
    }

    public int GetTime()
    {
        return Mathf.CeilToInt(currentTime); // Retorna el tiempo en enteros.
    }
}
