using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject asteroidPrefab;
    public float spawnInterval = 2f;
    public float spawnRangeX = 10f;
    public float spawnHeight = 10f;

    void Start()
    {
        InvokeRepeating("SpawnAsteroid", 1f, spawnInterval);
    }

    void SpawnAsteroid()
    {
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        Vector3 spawnPosition = new Vector3(randomX, spawnHeight, 0f);
        Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);
    }
}
