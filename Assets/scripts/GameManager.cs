using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject scoreController;
    private ScoreController sc;
    public GameObject shopKeeper;
    private Animator animator;
    public Transform[] spawnPoints;
    public GameObject prefab;

    // Lista para guardar los objetos instanciados
    private List<GameObject> spawnedTargets = new List<GameObject>();

    void Start()
    {
        sc = scoreController.GetComponent<ScoreController>();
        animator = shopKeeper.GetComponent<Animator>();

        SpawnAll();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            animator.CrossFade("Give", 0f);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            DestroyAll();
            SpawnAll();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            DestroyAll();
        }
    }

    // Método para instanciar en un punto aleatorio
    public void SpawnAtRandomPoint()
    {
        if (prefab != null && spawnPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[randomIndex];

            // Instancia el prefab y guárdalo en la lista
            GameObject newObject = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            spawnedTargets.Add(newObject);
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

            // Instancia el prefab y guárdalo en la lista
            GameObject newObject = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            spawnedTargets.Add(newObject);
        }
        else
        {
            Debug.LogWarning("Índice inválido o prefab/spawn points no asignados.");
        }
    }

    public void DestroyAll()
    {
        // Destruye todos los objetos en la lista
        foreach (GameObject obj in spawnedTargets)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }

        // Limpia la lista después de destruir los objetos
        spawnedTargets.Clear();
    }

    public void SpawnAll()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            GameObject newObject = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            spawnedTargets.Add(newObject);
        }
    }
}
