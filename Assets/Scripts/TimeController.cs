using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class timeController : MonoBehaviour
{
    private int time;
    public TextMeshPro timeText;
    // Start is called before the first frame update
    void Start()
    {
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateTimeText(int time)
    {
        timeText.text = time.ToString();
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

    public void SetTime(int points)
    {
        time = points;
        UpdateTimeText(time);
    }

    public int GetTime()
    {
        return time;
    }

}
