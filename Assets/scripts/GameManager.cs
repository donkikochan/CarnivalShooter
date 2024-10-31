using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    public GameObject scoreController;
    private ScoreController sc;
    // Start is called before the first frame update
    void Start()
    {
        sc = scoreController.GetComponent<ScoreController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
