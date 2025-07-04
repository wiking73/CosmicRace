using UnityEngine;
using System.Collections;

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject asteroidPrefab;
    [HideInInspector] public Transform playerTransform; // Będzie wypełniane automatycznie

    // Te zmienne są teraz mniej istotne, bo spawner będzie podążał za graczem
    // i zawsze będzie w "optymalnej" pozycji do spawnowania.
    // Zachowujemy je jako 'public', jeśli zechcesz je później wykorzystać do innych celów.
    public float detectionRadius = 50f; // Możesz je zignorować lub usunąć, jeśli niepotrzebne
    // public float spawnHeight = 50f; // Ta jest nadal ważna!
    // public float spawnForwardOffset = 30f; // Ta jest nadal ważna!
    // public float spawnSidewaysRange = 20f; // Ta jest nadal ważna!


    // Kluczowe parametry do regulacji spawnowania:
    [Header("Spawn Parameters")]
    public float spawnHeight = 50f; // Wysokość, z której spadają asteroidy
    public float spawnForwardOffset = 30f; // Jak daleko PRZED graczem ma się pojawić asteroida
    public float spawnSidewaysRange = 20f; // Zakres losowego położenia X względem GRACZA

    public float minSpawnInterval = 1f; // Minimalny czas między spawnami
    public float maxSpawnInterval = 3f; // Maksymalny czas między spawnami

    [Header("Asteroid Properties")]
    public float minAsteroidScale = 0.5f; // Minimalny rozmiar asteroidy
    public float maxAsteroidScale = 2.0f; // Maksymalny rozmiar asteroidy

    public float minInitialForce = 10f; // Minimalna początkowa siła (jeśli mają Rigidbody)
    public float maxInitialForce = 20f; // Maksymalna początkowa siła (jeśli mają Rigidbody)


    [Header("Asteroid Impact Properties")]
    public GameObject impactEffectPrefab;
    public AudioClip asteroidImpactSound;
    [Range(0.0f, 1.0f)]
    public float asteroidImpactSoundVolume = 1.0f;
    public AudioClip playerHitSound;
    [Range(0.0f, 1.0f)]
    public float playerHitSoundVolume = 1.0f;

    private float nextSpawnTime;
    private bool hasInitializedSpawnTime = false; // Flaga do jednokrotnej inicjalizacji czasu spawnu po znalezieniu gracza

    void Start()
    {
        Debug.Log("AsteroidSpawner: Spawner initialized. Waiting for player to be found for first spawn time setup.");
    }

    void Update()
    {
        // === Logika znajdowania gracza ===
        // To pozostaje, aby upewnić się, że playerTransform jest zawsze dostępny
        if (playerTransform == null)
        {
            GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
            if (playerGameObject != null)
            {
                playerTransform = playerGameObject.transform;
                Debug.Log("AsteroidSpawner: SUCCESSFULLY Found player: " + playerGameObject.name);
                
                // Po znalezieniu gracza, zainicjuj czas spawnu tylko raz
                if (!hasInitializedSpawnTime)
                {
                    SetNextSpawnTimeInternal(); // Ustawiamy pierwszy czas spawnu
                    hasInitializedSpawnTime = true;
                    Debug.Log($"AsteroidSpawner: Initial spawn time set to {nextSpawnTime} (Current Time: {Time.time}).");
                }
            }
            else
            {
                // To ostrzeżenie będzie się pojawiać w każdej klatce, dopóki gracz nie zostanie znaleziony.
                Debug.LogWarning("AsteroidSpawner: Player GameObject with tag 'Player' NOT FOUND in scene. Spawning paused until player is found.");
                return; // Wyjdź z Update, jeśli gracz nie jest jeszcze dostępny
            }
        }

        // Jeśli playerTransform jest nadal null (po próbie znalezienia), oznacza to, że gracz nie został znaleziony.
        // Wychodzimy, aby uniknąć NullReferenceException w dalszej części Update.
        if (playerTransform == null)
        {
            // Ten log będzie się pojawiał tylko, jeśli gracz nie zostanie znaleziony przez dłuższy czas
            Debug.LogWarning("AsteroidSpawner: PlayerTransform is still null, cannot proceed with spawning.");
            return;
        }

        if (Time.time >= nextSpawnTime)
        {
            SpawnAsteroid();
            SetNextSpawnTimeInternal(); // Ustawiamy następny czas spawnu
        }
    }

    // Prywatna metoda do ustawiania następnego czasu spawnu
    private void SetNextSpawnTimeInternal() 
    {
        nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    void SpawnAsteroid()
    {
        // Pozycja spawnu: przed graczem, na określonej wysokości, z losowym przesunięciem bocznym
        Vector3 spawnPosition = playerTransform.position + playerTransform.forward * spawnForwardOffset;
        spawnPosition.y = spawnHeight;
        spawnPosition.x += Random.Range(-spawnSidewaysRange, spawnSidewaysRange);
        Debug.Log($"AsteroidSpawner: Attempting to spawn asteroid at: {spawnPosition}. Relative to Player.");

        GameObject newAsteroid = Instantiate(asteroidPrefab, spawnPosition, Random.rotation);
        
        if (newAsteroid == null)
        {
            Debug.LogError("AsteroidSpawner: Failed to instantiate asteroidPrefab. Is it assigned in the Inspector?");
            return; // Zakończ, jeśli instancjonowanie się nie powiodło
        }

        float randomScale = Random.Range(minAsteroidScale, maxAsteroidScale);
        newAsteroid.transform.localScale = Vector3.one * randomScale;
        Debug.Log($"AsteroidSpawner: Asteroid spawned with scale: {randomScale}.");

        Rigidbody rb = newAsteroid.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 forceDirection = (Vector3.down * Random.Range(minInitialForce, maxInitialForce)) + 
                                     (Random.onUnitSphere * 0.5f); // Bardziej subtelne odchylenie
            forceDirection.y = -Mathf.Abs(forceDirection.y); // Upewniamy się, że zawsze leci w dół
            rb.AddForce(forceDirection, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse); // Losowy obrót
            Debug.Log($"AsteroidSpawner: Added force {forceDirection} and torque to asteroid Rigidbody.");
        }
        else
        {
            Debug.LogWarning("AsteroidSpawner: Asteroid prefab has no Rigidbody. It won't fall down naturally! Please add Rigidbody component to your asteroid prefab.");
        }

        AsteroidImpactHandler impactHandler = newAsteroid.AddComponent<AsteroidImpactHandler>();
        if (impactHandler != null)
        {
            impactHandler.impactSound = asteroidImpactSound; // PRZEKAZUJEMY DŹWIĘK Z SPANWERA
            impactHandler.impactSoundVolume = asteroidImpactSoundVolume; // PRZEKAZUJEMY GŁOŚNOŚĆ
            impactHandler.impactSound = playerHitSound; // PRZEKAZUJEMY DŹWIĘK Z SPANWERA
            impactHandler.impactSoundVolume = playerHitSoundVolume; // PRZEKAZUJEMY GŁOŚNOŚĆ
            impactHandler.impactEffectPrefab = impactEffectPrefab; // Przekazujemy prefab efektu, jeśli nadal go używasz
            Debug.Log("AsteroidSpawner: AsteroidImpactHandler added and configured to new asteroid.");
        }
        else
        {
            Debug.LogError("AsteroidSpawner: Failed to add AsteroidImpactHandler to new asteroid! Check if the script exists.");
        }
    }
}