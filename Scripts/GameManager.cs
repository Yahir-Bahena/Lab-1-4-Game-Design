using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private int playerLives = 3;
    [SerializeField] private int balloonsToWin = 10;
    
    [Header("Scoring")]
    [SerializeField] private int basePoints = 100;
    [SerializeField] private int bonusPointsPerSize = 50;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject balloonPrefab;
    
    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-8, 3);
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(8, 5);
    
    private int currentLives;
    private int balloonsPopped = 0;
    private int currentScore = 0;
    private float spawnTimer;
    private Camera mainCamera;

    public static GameManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentLives = playerLives;
        currentScore = 0;
        mainCamera = Camera.main;
        spawnTimer = spawnInterval;
        
        Debug.Log($"Game Started! Lives: {currentLives}, Score: {currentScore}");
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            SpawnBalloon();
            spawnTimer = spawnInterval;
        }
    }

    void SpawnBalloon()
    {
        if (balloonPrefab == null)
        {
            Debug.LogWarning("Balloon Prefab not assigned to GameManager!");
            return;
        }

        float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float randomY = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        Vector3 spawnPosition = new Vector3(randomX, randomY, 0);

        Instantiate(balloonPrefab, spawnPosition, Quaternion.identity);
    }

    public void BalloonPopped(float balloonSize)
    {
        balloonsPopped++;
        
        int sizeBonus = Mathf.RoundToInt(bonusPointsPerSize / balloonSize);
        int points = basePoints + sizeBonus;
        
        currentScore += points;
        
        Debug.Log($"Balloon popped! +{points} points (Size: {balloonSize:F2}x). Total Score: {currentScore}");
        Debug.Log($"Balloons popped: {balloonsPopped}/{balloonsToWin}");

        if (balloonsPopped >= balloonsToWin)
        {
            WinGame();
        }
    }
    
    public void BalloonEscaped()
    {
        Debug.Log("Balloon escaped! No points awarded.");
        LoseLife();
    }

    public void LoseLife()
    {
        currentLives--;
        Debug.Log($"Life lost! Remaining lives: {currentLives}");

        if (currentLives <= 0)
        {
            GameOver();
        }
    }

    void WinGame()
    {
        Debug.Log("YOU WIN!");
        Invoke("RestartGame", 2f);
    }

    void GameOver()
    {
        Debug.Log("GAME OVER!");
        Invoke("RestartGame", 2f);
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public int GetLives()
    {
        return currentLives;
    }

    public int GetBalloonsPopped()
    {
        return balloonsPopped;
    }
    
    public int GetScore()
    {
        return currentScore;
    }
}