using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float startDelay = 3f;
    private float timer = 0f;
    public bool raceStarted = false;

    public TextMeshProUGUI countdownText;

    [HideInInspector] 
    public GameObject playerCarInstance;
    
    public Vector3 defaultSpawnPosition = new Vector3(4669.7f, 11.3f, 2133.91f);
    public Vector3 defaultSpawnRotationEuler = new Vector3(0f, -840.477f, 0f);


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return; 
        }

        Vector3 spawnPosition;
        Quaternion spawnRotation;

        GameObject existingPlayer = GameObject.FindWithTag("Player"); 

        if (existingPlayer != null)
        {
            spawnPosition = existingPlayer.transform.position;
            Vector3 existingEuler = existingPlayer.transform.rotation.eulerAngles;
            spawnRotation = Quaternion.Euler(0f, existingEuler.y, 0f);
    
            Destroy(existingPlayer); 
        }
        else
        {
            spawnPosition = defaultSpawnPosition;
            spawnRotation = Quaternion.Euler(0f, defaultSpawnRotationEuler.y, 0f);
            Debug.LogWarning("GameManager: No 'Player' object with tag 'Player' found. Spawning at default position from GameManager inspector.");
        }

        if (VehicleSelectManager.SelectedCarPrefab != null)
        {
            playerCarInstance = Instantiate(
                VehicleSelectManager.SelectedCarPrefab,
                spawnPosition,
                spawnRotation);

            playerCarInstance.name = "PlayerCar";
            Position positionScript = FindObjectOfType<Position>();
            if (positionScript != null)
            {
                if (positionScript.allCars == null)
                {
                    positionScript.allCars = new List<Transform>();
                }
                positionScript.allCars.Add(playerCarInstance.transform);
            }

        }
        else
        {
            Debug.LogError("GameManager: SelectedCarPrefab is null! Cannot spawn car. Check VehicleSelectManager.");
        }
    }

    void Update()
    {
        if (!raceStarted)
        {
            timer += Time.deltaTime;
            float remainingTime = startDelay - timer;

            if (remainingTime > 0)
            {
                countdownText.text = Mathf.Ceil(remainingTime).ToString();
            }
            else
            {
                countdownText.text = "START!";
                raceStarted = true;
                Invoke("HideCountdownText", 1f);
                Debug.Log("Race is started!");
            }
        }
    }

    void HideCountdownText()
    {
        countdownText.gameObject.SetActive(false);
    }
}
