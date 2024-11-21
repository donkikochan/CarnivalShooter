using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StreakController : MonoBehaviour
{
    private int streak;
    public TextMeshPro streakText;
    // Start is called before the first frame update
    void Start()
    {
        streak = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateStreakText(int streak)
    {
        streakText.text = streak.ToString();
    }

    public void AddStreak(int streak)
    {
        this.streak += streak;
        UpdateStreakText(streak);
    }

    public void RemoveStreak(int streak)
    {
        this.streak -= streak;
        UpdateStreakText(streak);
    }

    public void ResetStreak()
    {
        streak = 0;
        UpdateStreakText(streak);
    }

    public void SetStreak(int streak)
    {
        this.streak = streak;
        UpdateStreakText(streak);
    }

    public int GetStreak()
    {
        return streak;
    }

}
