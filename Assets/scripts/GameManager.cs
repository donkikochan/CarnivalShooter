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
    public Transform[] spawnPoints;
    public GameObject prefab;
    
    void Start()
    {
        sc = scoreController.GetComponent<ScoreController>();
        animator = shopKeeper.GetComponent<Animator>();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            SpawnAtPoint(i);
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            animator.CrossFade("Give", 0f);
        }
    }
    
    // Método para instanciar en un punto aleatorio
    public void SpawnAtRandomPoint()
    {
        if (prefab != null && spawnPoints.Length > 0)
        {
            // Elige un spawn point aleatorio
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[randomIndex];

            // Instancia el prefab en el spawn point
            Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            Debug.LogWarning("Faltan prefabs o spawn points.");
        }
    }

    // Método opcional para instanciar en un punto específico
    public void SpawnAtPoint(int index)
    {
        if (prefab != null && index >= 0 && index < spawnPoints.Length)
        {
            Transform spawnPoint = spawnPoints[index];
            Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            Debug.LogWarning("Índice inválido o prefab/spawn points no asignados.");
        }
    }
}
