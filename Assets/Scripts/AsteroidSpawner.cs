using UnityEngine;
using System.Collections;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Link do gracza")]
    private Transform playerTransform;

    [Header("Prefab & Parametry spawnu")]
    public GameObject asteroidPrefab;
    public float spawnHeight = 50f;
    public float spawnForwardOffset = 30f;
    public float spawnSidewaysRange = 20f;
    public float minSpawnInterval = 1f;
    public float maxSpawnInterval = 3f;

    [Header("Właściwości asteroidy")]
    public float minAsteroidScale = 0.5f;
    public float maxAsteroidScale = 2.0f;
    public float minInitialForce = 10f;
    public float maxInitialForce = 20f;

    [Header("Efekty & dźwięki uderzeń")]
    public GameObject impactEffectPrefab;
    public AudioClip asteroidImpactSound;
    [Range(0f, 1f)] public float asteroidImpactSoundVolume = 1f;
    public AudioClip playerHitSound;
    [Range(0f, 1f)] public float playerHitSoundVolume = 1f;

    private float nextSpawnTime;
    private bool spawnTimeInitialized = false;

    // Dodatkowa flaga, aby zidentyfikować, czy ten spawner jest "głównym"
    // To pole jest domyślnie publiczne i widoczne w Inspektorze
    // ZAZNACZ JE TYLKO NA TYM SPANWERZE, KTÓRY MA SPAWNOWAĆ ASTEROIDY
    public bool isMainSpawner = false; 

    void Start()
    {
        // Jeśli ten skrypt NIE jest głównym spawnerem (czyli jest na instancjonowanej asteroidzie)
        // to powinien się dezaktywować.
        if (!isMainSpawner)
        {
            this.enabled = false; // Dezaktywuje ten konkretny komponent AsteroidSpawner
            Debug.LogWarning("AsteroidSpawner: Non-main spawner detected on an asteroid. Deactivating its spawner logic.");
            return; // Zakończ, bo ten spawner nie ma działać
        }

        Debug.Log("AsteroidSpawner: Main Spawner initialized. Waiting for player to be found for first spawn time setup.");
    }

    void Update()
    {
        // Tylko główny spawner ma wykonywać logikę spawnowania
        if (!isMainSpawner)
        {
            return; // Upewnij się, że inne spawny są wyłączone w Update też
        }

        // 1) Znajdź gracza, jeśli jeszcze nie mamy
        if (playerTransform == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go != null)
            {
                playerTransform = go.transform;
                Debug.Log("[AsteroidSpawner] Player found, initializing spawn timer.");
                ScheduleNextSpawn();
                spawnTimeInitialized = true;
            }
            else
            {
                return;
            }
        }

        // 2) Jeśli już zainicjowaliśmy timer i jest czas — spawnuj
        if (spawnTimeInitialized && Time.time >= nextSpawnTime)
        {
            SpawnAsteroid();
            ScheduleNextSpawn();
        }
    }

    private void ScheduleNextSpawn()
    {
        nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
        Debug.Log($"[AsteroidSpawner] Next spawn in {nextSpawnTime - Time.time:F2}s (at t = {nextSpawnTime:F2})");
    }

    private void SpawnAsteroid()
    {
        // 1) Oblicz “płaskie” wektory forward/right względem świata,
        Vector3 flatForward = Vector3.ProjectOnPlane(playerTransform.forward, Vector3.up).normalized;
        Vector3 flatRight   = Vector3.Cross(Vector3.up, flatForward).normalized;

        float forwardDist = spawnForwardOffset;

        float sideOffset = Random.Range(-spawnSidewaysRange, spawnSidewaysRange);

        // 4) Docelowa pozycja:
        Vector3 spawnPos = playerTransform.position
                         + flatForward * forwardDist
                         + flatRight   * sideOffset
                         + Vector3.up  * spawnHeight;

        Debug.Log($"[AsteroidSpawner] Spawning at {spawnPos}");

        // 5) Instancja:
        GameObject a = Instantiate(asteroidPrefab, spawnPos, Random.rotation);
        a.transform.localScale = Vector3.one * Random.Range(minAsteroidScale, maxAsteroidScale);

        // 6) RigidBody jako ciężki kamień:
        if (a.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.mass = 50f;
            rb.useGravity = true;
            rb.drag = 0f;
            rb.angularDrag = 0f;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            Vector3 sideForce = new Vector3(Random.Range(-1f,1f), 0f, Random.Range(-1f,1f)) * 2f;
            Vector3 force = Vector3.down * Random.Range(minInitialForce, maxInitialForce) + sideForce;
            rb.AddForce(force, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning("[AsteroidSpawner] Prefab ma braki Rigidbody!");
        }

        // 7) Podczep handler kolizji i przekaz parametry:
        var h = a.AddComponent<AsteroidImpactHandler>();
        h.impactEffectPrefab   = impactEffectPrefab;
        h.impactSound          = asteroidImpactSound;
        h.impactSoundVolume    = asteroidImpactSoundVolume;
        h.playerHitSound       = playerHitSound;
        h.playerHitSoundVolume = playerHitSoundVolume;

        // **NOWA LOGIKA:** Jeśli prefab asteroidy zawiera również AsteroidSpawner, dezaktywuj go.
        // To jest kluczowe, aby instancjonowane asteroidy nie stawały się nowymi spawnerami.
        if (a.TryGetComponent<AsteroidSpawner>(out var spawnedSpawner))
        {
            spawnedSpawner.enabled = false; // Dezaktywuj skrypt AsteroidSpawner na tej asteroidzie
            Debug.Log($"[AsteroidSpawner] Deactivated AsteroidSpawner on spawned asteroid '{a.name}'.");
        }
    }
}