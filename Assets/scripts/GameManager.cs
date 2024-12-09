using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    STATE_MENU,
    STATE_BEGINGAME,
    STATE_PLAYING,
    STATE_ENDGAME
}

public class GameManager : MonoBehaviour
{
    // Configuración de Prefabs
    [Header("Prefabs")]
    public GameObject bandido;
    public GameObject armero;
    public GameObject secuaz;
    public GameObject crio;
    public GameObject damisela;
    public GameObject banquero;
    public GameObject shopKeeper;

    // Probabilidades de aparición
    [Header("Probabilidades de Aparición")]
    [Range(0, 100)] public int bandidoChance = 70;
    [Range(0, 100)] public int armeroChance = 30;
    [Range(0, 100)] public int secuazChance = 50;
    [Range(0, 100)] public int crioChance = 15;
    [Range(0, 100)] public int damiselaChance = 10;
    [Range(0, 100)] public int banqueroChance = 30;
    [Range(1, 2)] public float spawnSpeed = 1;

    // Configuración de puntos de spawn
    [Header("Puntos de Spawn")]
    public Transform[] spawnPoints;

    // Controladores
    [Header("Controladores")]
    public GameObject scoreController;

    // Configuración de oleadas
    [Header("Configuración de Oleadas")]
    [Tooltip("Tiempo entre oleadas (segundos)")]
    public float waveInterval = 10f; // Tiempo entre oleadas
    [Tooltip("Cantidad de enemigos por oleada")]
    public int enemiesPerWave = 5; // Enemigos por oleada
    public float spawnDelay = 0.5f; // Retraso entre spawns individuales en una oleada

    [Header("Game State")] 
    public GameState gameState;

    // Lista para guardar los objetos instanciados
    private List<GameObject> spawnedTargets = new List<GameObject>();

    // Control de puntos ocupados
    private HashSet<int> occupiedSpawnPoints = new HashSet<int>();

    private ScoreController sc;
    private Animator animator;

    void Start()
    {
        sc = scoreController.GetComponent<ScoreController>();
        animator = shopKeeper.GetComponent<Animator>();

        StartCoroutine(SpawnWaves());
    }

    void Update()
    {
        switch (gameState)
        {
            case GameState.STATE_MENU:
            {
                break;
            }
            case GameState.STATE_BEGINGAME:
            {
                break;
            }
            case GameState.STATE_PLAYING:
            {
                break;
            }
            case GameState.STATE_ENDGAME:
            {
                break;
            }
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            animator.CrossFade("Give", 0f);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            DestroyAll();
        }
    }

    // Corrutina para manejar oleadas
    private IEnumerator SpawnWaves()
    {
        while (true)
        {
            yield return StartCoroutine(SpawnWave());
            yield return new WaitForSeconds(waveInterval);
        }
    }

    // Corrutina para generar una oleada
    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            SpawnAtRandomPoint();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    // Método para instanciar en un punto aleatorio
    public void SpawnAtRandomPoint()
    {
        if (spawnPoints.Length > 0)
        {
            int randomIndex = GetAvailableSpawnPoint();
            if (randomIndex >= 0)
            {
                Transform spawnPoint = spawnPoints[randomIndex];
                GameObject prefab = GetRandomPrefab();

                // Instancia el prefab
                GameObject newObject = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
                spawnedTargets.Add(newObject);

                // Marca el punto como ocupado
                occupiedSpawnPoints.Add(randomIndex);

                // Liberar el punto cuando el objeto se destruya
                newObject.GetComponent<Objectives>().OnDestroyed += () => ReleaseSpawnPoint(randomIndex);
            }
        }
        else
        {
            Debug.LogWarning("Faltan spawn points.");
        }
    }

// Método para obtener un prefab basado en las probabilidades
    private GameObject GetRandomPrefab()
    {
        // Suma todas las probabilidades
        int totalChance = bandidoChance + armeroChance + secuazChance + crioChance + damiselaChance + banqueroChance;
        int randomValue = Random.Range(0, totalChance);

        // Selección en función del rango de probabilidades
        if (randomValue < bandidoChance)
        {
            return bandido;
        }
        else if (randomValue < bandidoChance + armeroChance)
        {
            return armero;
        }
        else if (randomValue < bandidoChance + armeroChance + secuazChance)
        {
            return secuaz;
        }
        else if (randomValue < bandidoChance + armeroChance + secuazChance + crioChance)
        {
            return crio;
        }
        else if (randomValue < bandidoChance + armeroChance + secuazChance + crioChance + damiselaChance)
        {
            return damisela;
        }
        else
        {
            return banquero;
        }
    }

    // Método para obtener un índice de punto de spawn disponible
    private int GetAvailableSpawnPoint()
    {
        List<int> availablePoints = new List<int>();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (!occupiedSpawnPoints.Contains(i))
            {
                availablePoints.Add(i);
            }
        }

        if (availablePoints.Count > 0)
        {
            return availablePoints[Random.Range(0, availablePoints.Count)];
        }

        Debug.LogWarning("No hay puntos de spawn disponibles.");
        return -1;
    }

    // Método para liberar un punto de spawn
    private void ReleaseSpawnPoint(int index)
    {
        occupiedSpawnPoints.Remove(index);
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

        // Limpia la lista y los puntos ocupados
        spawnedTargets.Clear();
        occupiedSpawnPoints.Clear();
    }

    public void SetAnimatonSpeed(float speed)
    {
        spawnSpeed = speed;
        animator.speed = spawnSpeed;
    }
}
