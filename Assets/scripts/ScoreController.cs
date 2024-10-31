using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreController : MonoBehaviour
{
    private int score;
    public TextMeshPro scoreText;
    // Start is called before the first frame update
    void Start()
    {
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScoreText(int score)
    {
        scoreText.text = score.ToString();
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText(score);
    }

    public void RemoveScore(int points)
    {
        score -= points;
        UpdateScoreText(score);
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreText(score);
    }

    public void SetScore(int points)
    {
        score = points;
        UpdateScoreText(score);
    }

    public int GetScore()
    {
        return score;
    }

}
