using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    public GameObject scoreController;
    private ScoreController sc;
    public GameObject shopKeeper;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        sc = scoreController.GetComponent<ScoreController>();
        animator = shopKeeper.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            animator.CrossFade("Give", 0f);
        }
    }

}
