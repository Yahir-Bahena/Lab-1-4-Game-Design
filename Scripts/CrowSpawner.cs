using UnityEngine;

public class CrowSpawner : MonoBehaviour
{
    public GameObject crowPrefab;
    public float minSpawnInterval = 5f;
    public float maxSpawnInterval = 12f;

    public float spawnHeight = 0f;
    public float spawnOffsetX = 1f;

    private float nextSpawnTime;
    private Camera mainCamera;
    private GameObject currentCrow;

    void Start()
    {
        mainCamera = Camera.main;
        ScheduleNextSpawn();
    }

    void Update()
    {
        if (currentCrow == null && Time.time >= nextSpawnTime)
        {
            SpawnCrow();
        }
    }

    private void SpawnCrow()
    {
        if (crowPrefab == null)
        {
            Debug.LogWarning("Crow Prefab not assigned!");
            return;
        }

        bool spawnFromLeft = Random.value > 0.5f;

        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

        Vector3 spawnPosition;
        bool movingRight;

        if (spawnFromLeft)
        {
            spawnPosition = new Vector3(bottomLeft.x - spawnOffsetX, spawnHeight, 0);
            movingRight = true;
        }
        else
        {
            spawnPosition = new Vector3(topRight.x + spawnOffsetX, spawnHeight, 0);
            movingRight = false;
        }

        GameObject crow = Instantiate(crowPrefab, spawnPosition, Quaternion.identity);
        currentCrow = crow;

        var crowMove = crow.GetComponent<CrowMovement>();
        if (crowMove != null)
        {
            crowMove.SetDirection(movingRight);

            // subscribe to callback so we spawn the next AFTER this crow is gone
            crowMove.onDespawn = HandleCrowDespawn;
        }
    }

    private void HandleCrowDespawn()
    {
        currentCrow = null;
        ScheduleNextSpawn();
    }

    private void ScheduleNextSpawn()
    {
        float randomInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
        nextSpawnTime = Time.time + randomInterval;
    }
}
