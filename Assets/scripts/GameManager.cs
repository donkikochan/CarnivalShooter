using System.Collections;
using System.Collections.Generic;
using Autohand;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    STATE_MENU,
    STATE_BEGINGAME,
    STATE_PLAYING,
    STATE_ENDGAME
}

public enum PreviewWeapon
{
    DEAGLE,
    SVD,
    SPAS
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
    public Transform[] coinSpawnPoints;

    // Controladores
    [Header("Controladores")]
    public GameObject scoreController;
    public GameObject timeController;

    // Configuración de oleadas
    [Header("Configuración de Oleadas")]
    [Tooltip("Tiempo entre oleadas (segundos)")]
    public float waveInterval = 10f; // Tiempo entre oleadas
    [Tooltip("Cantidad de enemigos por oleada")]
    public int enemiesPerWave = 5; // Enemigos por oleada
    public float spawnDelay = 0.5f; // Retraso entre spawns individuales en una oleada

    [Header("Objects")] 
    public PlacePoint coinPlacePoint;
    public GameObject[] coins;
    
    [Header("Select Weapon")]
    public GameObject selectWeaponState;
    public Transform previewWeaponPoint;
    public GameObject[] previewWeapons;
    public Transform playerWeaponPoint;
    public GameObject[] playerWeapons;
    public Transform playerMagazinePoint;
    public GameObject[] playerMagazines;
    public GameObject gameOverState;

    [Header("Start Game")] 
    public GameObject cartel;
    private bool isAnimating;

    // Lista para guardar los objetos instanciados
    private List<GameObject> spawnedTargets = new List<GameObject>();

    // Control de puntos ocupados
    private HashSet<int> occupiedSpawnPoints = new HashSet<int>();

    private GameState gameState = GameState.STATE_MENU;
    private TimeController tc;
    private GameObject[] instCoins = new GameObject[3];
    private PreviewWeapon currentPreviewWeapon = PreviewWeapon.DEAGLE;
    private GameObject instPreviewWeapon;
    private bool needPreviewWeaponUpdate = true;
    private GameObject instPlayerWeapon;
    private GameObject instPlayerMagazine;
    private bool startCoroutineGame = true;
    private float startGameCountDown = 5f;
    private int givesTimePositive = 10;
    private int givesTimeNegative = 10;
    private Coroutine spawnWavesCoroutine;
    private bool showGameOver = true;
    
    [Header("Sound Controller")]
    public GameObject Ambience;
    public GameObject GameAmbience;

    void Start()
    {
        tc = timeController.GetComponent<TimeController>();
        Ambience.SetActive(true);
        GameAmbience.SetActive(false);
    }

    void Update()
    {
        switch (gameState)
        {
            case GameState.STATE_MENU:
            {
                // Instancia solo si no se han generado las monedas
                if (!instCoins[0])
                    instCoins[0] = Instantiate(coins[0], coinSpawnPoints[0].position, coinSpawnPoints[0].rotation);
                if (!instCoins[1])
                    instCoins[1] = Instantiate(coins[1], coinSpawnPoints[1].position, coinSpawnPoints[1].rotation);
                if (!instCoins[2])
                    instCoins[2] = Instantiate(coins[2], coinSpawnPoints[2].position, coinSpawnPoints[2].rotation);

                UpdateCartel("Grab a coin:\n\nGold = Easy mode\n\nSilver = Medium mode\n\nCopper = Hard mode");
                
                break;
            }
            case GameState.STATE_BEGINGAME:
            {
                if (instCoins[0] != null && GetPlacedCoin() != 1)
                    Destroy(instCoins[0]);
                if (instCoins[1] != null && GetPlacedCoin() != 2)
                    Destroy(instCoins[1]);
                if (instCoins[2] != null && GetPlacedCoin() != 3)
                    Destroy(instCoins[2]);
                
                if (!selectWeaponState.activeSelf)
                    selectWeaponState.SetActive(true);
                
                UpdateCartel("Use the Lever to\n\nchange Weapon\n\nPress the Red Button\n\nto chose it");

                UpdatePreviewWeapon();
                
                Debug.Log("La monedica es: " + GetPlacedCoin());
                break;
            }
            case GameState.STATE_PLAYING:
            {
                if (instPreviewWeapon != null)
                    Destroy(instPreviewWeapon);

                if (!instPlayerWeapon)
                    instPlayerWeapon = Instantiate(playerWeapons[(int)currentPreviewWeapon], playerWeaponPoint.position, playerWeaponPoint.rotation);
                if (!instPlayerMagazine)
                    instPlayerMagazine = Instantiate(playerMagazines[(int)currentPreviewWeapon], playerMagazinePoint.position, playerMagazinePoint.rotation);
                
                if (startCoroutineGame)
                {
                    ChangeCartelText("Game Starts in: \n\n" + Mathf.Max(0, Mathf.Ceil(startGameCountDown)) + " seconds");
                    startGameCountDown -= Time.deltaTime;
                    Invoke("StartGame", 5.0f);
                    
                    Ambience.SetActive(false);
                    GameAmbience.SetActive(true);
                   
                }
                
                if (tc.HasEnded())
                {
                    gameState = GameState.STATE_ENDGAME;
                    break;
                }
                
                AdjustSpawnProbabilities();
                
                break;
            }
            case GameState.STATE_ENDGAME:
            {
                StopCoroutine(spawnWavesCoroutine);
                if (showGameOver)
                {
                    ChangeCartelText("The Game is Over\n\nPress the button\n\nfor restart game");
                    SetCartelState(true);
                    gameOverState.SetActive(true);
                    showGameOver = false;
                }
                break;
            }
        }
        
        Debug.Log(gameState);
        /*if (Input.GetKeyDown(KeyCode.A))
        {
            animator.CrossFade("Give", 0f);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            DestroyAll();
        }*/
    }

    private void StartGame()
    {
        if (!startCoroutineGame)
            return;
        
        spawnWavesCoroutine = StartCoroutine(SpawnWaves());
        SetCartelState(false);
        startCoroutineGame = false;
        tc.beginTimer = true;
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
                newObject.GetComponent<Animator>().speed = spawnSpeed;
                Objectives objObjective = newObject.GetComponent<Objectives>();
                if (objObjective.givesNegativeTime)
                    objObjective.timeThatGives = givesTimeNegative;
                else
                    objObjective.timeThatGives = givesTimePositive;
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

    public void CoinPlaced()
    {
        gameState = GameState.STATE_BEGINGAME;
    }

    public void WeaponSelected()
    {
        selectWeaponState.SetActive(false);
        gameState = GameState.STATE_PLAYING;
    }

    public int GetPlacedCoin()
    {
        switch (coinPlacePoint.placedObject.gameObject.name)
        {
            case "GoldCoin(Clone)":
            {
                return 1;
            }
            case "SilverCoin(Clone)":
            {
                return 2;
            }
            case "CopperCoin(Clone)":
            {
                return 3;
            }
        }

        return 0;
    }

    private void UpdatePreviewWeapon()
    {
        if (!needPreviewWeaponUpdate)
            return;
        
        if (!instPreviewWeapon)
        {
            instPreviewWeapon = Instantiate(previewWeapons[(int)currentPreviewWeapon], previewWeaponPoint.position, previewWeapons[(int)currentPreviewWeapon].transform.rotation);
        }
        else
        {
            Destroy(instPreviewWeapon);
            instPreviewWeapon = Instantiate(previewWeapons[(int)currentPreviewWeapon], previewWeaponPoint.position, previewWeapons[(int)currentPreviewWeapon].transform.rotation);
        }

        needPreviewWeaponUpdate = false;
    }

    public void LeftChangeWeapon()
    {
        switch (currentPreviewWeapon)
        {
            case PreviewWeapon.DEAGLE:
            {
                currentPreviewWeapon = PreviewWeapon.SPAS;
                break;
            }
            case PreviewWeapon.SVD:
            {
                currentPreviewWeapon = PreviewWeapon.DEAGLE;
                break;
            }
            case PreviewWeapon.SPAS:
            {
                currentPreviewWeapon = PreviewWeapon.SVD;
                break;
            }
        }

        needPreviewWeaponUpdate = true;
    }

    public void RightChangeWeapon()
    {
        switch (currentPreviewWeapon)
        {
            case PreviewWeapon.DEAGLE:
            {
                currentPreviewWeapon = PreviewWeapon.SVD;
                break;
            }
            case PreviewWeapon.SVD:
            {
                currentPreviewWeapon = PreviewWeapon.SPAS;
                break;
            }
            case PreviewWeapon.SPAS:
            {
                currentPreviewWeapon = PreviewWeapon.DEAGLE;
                break;
            }
        }

        needPreviewWeaponUpdate = true;
    }
    
    private void AdjustSpawnProbabilities()
    {
        int minutesRemaining = Mathf.FloorToInt(tc.GetTime() / 60); // Obtenemos los minutos restantes
        int splitTime = tc.time / 3;
        

        switch (GetPlacedCoin())
        {
            case 1: // GoldCoin (Fácil)
                if (minutesRemaining <= (splitTime * 3) && minutesRemaining > (splitTime * 2))
                {
                    SetEasyGoldPhase();
                }
                else if (minutesRemaining <= (splitTime * 2) && minutesRemaining > splitTime)
                {
                    SetModerateGoldPhase();
                }
                else if (minutesRemaining <= splitTime)
                {
                    SetHardGoldPhase();
                }
                break;

            case 2: // SilverCoin (Media)
                if (minutesRemaining <= (splitTime * 3) && minutesRemaining > (splitTime * 2))
                {
                    SetEasySilverPhase();
                }
                else if (minutesRemaining <= (splitTime * 2) && minutesRemaining > splitTime)
                {
                    SetModerateSilverPhase();
                }
                else if (minutesRemaining <= splitTime)
                {
                    SetHardSilverPhase();
                }
                break;

            case 3: // CopperCoin (Difícil)
                if (minutesRemaining <= (splitTime * 3) && minutesRemaining > (splitTime * 2))
                {
                    SetEasyCopperPhase();
                }
                else if (minutesRemaining <= (splitTime * 2) && minutesRemaining > splitTime)
                {
                    SetModerateCopperPhase();
                }
                else if (minutesRemaining <= splitTime)
                {
                    SetHardCopperPhase();
                }
                break;
        }
    }
    
    private void ChangeCartelText(string newText)
    {
        TextMeshPro cartelText = cartel.GetComponentInChildren<TextMeshPro>();

        if (!cartelText)
            return;

        
        if (cartelText.text != newText)
        {
            cartelText.text = newText;
        }
    }

    private void SetCartelState(bool state)
    {
        Animator cartelAnimator = cartel.GetComponent<Animator>();
        
        cartelAnimator.SetTrigger(state ? "Open" : "Close");
    }

    private IEnumerator ChangeCartel(string newText)
    {
        TextMeshPro cartelText = cartel.GetComponentInChildren<TextMeshPro>();
        Animator cartelAnimator = cartel.GetComponent<Animator>();
        
        if (isAnimating)
            yield break;

        isAnimating = true;

        if (!cartelText || !cartelAnimator || cartelText.text == newText)
        {
            isAnimating = false;
            yield break;
        }
        
        cartelAnimator.SetTrigger("Close");

        yield return new WaitForSeconds(cartelAnimator.GetCurrentAnimatorStateInfo(0).length);
        
        cartelText.text = newText;
        
        cartelAnimator.SetTrigger("Open");

        isAnimating = false;
    }
    
    public void UpdateCartel(string text)
    {
        StartCoroutine(ChangeCartel(text));
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    #region GoldCoin (Fácil)

    private void SetEasyGoldPhase()
    {
        bandidoChance = 50; 
        armeroChance = 20; 
        secuazChance = 40; 
        crioChance = 10; 
        damiselaChance = 10; 
        banqueroChance = 20; 
        spawnSpeed = 0.8f; 
        enemiesPerWave = Random.Range(4, 8); 
        waveInterval = Random.Range(5f, 7f);
        givesTimePositive = 10;
        givesTimePositive = 10;
    }

    private void SetModerateGoldPhase()
    {
        bandidoChance = 60; 
        armeroChance = 30; 
        secuazChance = 50; 
        crioChance = 15; 
        damiselaChance = 15; 
        banqueroChance = 30; 
        spawnSpeed = 1.0f; 
        enemiesPerWave = Random.Range(6, 12); 
        waveInterval = Random.Range(4f, 6f);
        givesTimePositive = 10;
        givesTimeNegative = 10;
    }

    private void SetHardGoldPhase()
    {
        bandidoChance = 70; 
        armeroChance = 40; 
        secuazChance = 60; 
        crioChance = 20; 
        damiselaChance = 20; 
        banqueroChance = 40; 
        spawnSpeed = 1.2f; 
        enemiesPerWave = Random.Range(8, 14); 
        waveInterval = Random.Range(3f, 5f);
        givesTimePositive = 10;
        givesTimeNegative = 10;
    }
    #endregion

    #region SilverCoin (Media)
    private void SetEasySilverPhase() 
    { 
        bandidoChance = 40; 
        armeroChance = 30; 
        secuazChance = 50; 
        crioChance = 10; 
        damiselaChance = 15; 
        banqueroChance = 25; 
        spawnSpeed = 1.0f; 
        enemiesPerWave = Random.Range(5, 10); 
        waveInterval = Random.Range(4f, 6f);
        givesTimePositive = 7;
        givesTimeNegative = 7;
    }

    private void SetModerateSilverPhase()
    {
        bandidoChance = 60; 
        armeroChance = 40; 
        secuazChance = 60; 
        crioChance = 20; 
        damiselaChance = 20; 
        banqueroChance = 30; 
        spawnSpeed = 1.2f; 
        enemiesPerWave = Random.Range(10, 14); 
        waveInterval = Random.Range(3f, 5f);
        givesTimePositive = 7;
        givesTimeNegative = 7;
    }

    private void SetHardSilverPhase()
    {
        bandidoChance = 70; 
        armeroChance = 50; 
        secuazChance = 70; 
        crioChance = 30; 
        damiselaChance = 25; 
        banqueroChance = 40; 
        spawnSpeed = 1.5f; 
        enemiesPerWave = Random.Range(12, 16); 
        waveInterval = Random.Range(2f, 4f);
        givesTimePositive = 7;
        givesTimeNegative = 7;
    }
    #endregion

    #region CopperCoin (Difícil)

    private void SetEasyCopperPhase()
    {
        bandidoChance = 30; 
        armeroChance = 40; 
        secuazChance = 60; 
        crioChance = 10; 
        damiselaChance = 15; 
        banqueroChance = 20; 
        spawnSpeed = 1.0f; 
        enemiesPerWave = Random.Range(6, 12); 
        waveInterval = Random.Range(3f, 5f);
        givesTimePositive = 4;
        givesTimeNegative = 4;
    }

    private void SetModerateCopperPhase()
    {
        bandidoChance = 50; 
        armeroChance = 60; 
        secuazChance = 70; 
        crioChance = 25; 
        damiselaChance = 25; 
        banqueroChance = 35; 
        spawnSpeed = 1.2f; 
        enemiesPerWave = Random.Range(12, 16); 
        waveInterval = Random.Range(2f, 4f);
        givesTimePositive = 4;
        givesTimeNegative = 4;
    }

    private void SetHardCopperPhase()
    {
        bandidoChance = 80; 
        armeroChance = 80; 
        secuazChance = 90; 
        crioChance = 40; 
        damiselaChance = 35; 
        banqueroChance = 50; 
        spawnSpeed = 1.5f; 
        enemiesPerWave = Random.Range(16, 18); 
        waveInterval = Random.Range(1f, 3f);
        givesTimePositive = 4;
        givesTimeNegative = 4;
    }
    #endregion
}
