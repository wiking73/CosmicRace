using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float startDelay = 3f;
    private float timer = 0f;
    public bool raceStarted = false;

    public TextMeshProUGUI countdownText;

    public vehicleList list;
    public GameObject startPosition;
    // private CarController RR;

    private void Awake()
    {
        Instantiate(
            list.vehicles[PlayerPrefs.GetInt("VehiclePointer")],
            startPosition.transform.position,
            startPosition.transform.rotation);

        // RR = GameObject.FindGameObjectWithTag("Player").GetComponent<CarController>();

        // Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
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
