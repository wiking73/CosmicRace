using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]
public class FinishResult
{
    public string racerName;
    public float finishTime;
}

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance;

    public List<Transform> activeRacers = new List<Transform>();
    public Text positionText;
    public Text resultText;
    public GameObject resultPanel;
    public UnityEngine.UI.Text countdownText;
    public TimeCounter timeCounter;
    private bool raceFinished = false;
    public List<FinishResult> finishOrder = new List<FinishResult>();
    public Position positionManager;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        GameObject[] playerCars = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] aiCars = GameObject.FindGameObjectsWithTag("AI");
        
        activeRacers.Clear(); 

        foreach (GameObject go in playerCars)
        {
            if (go != null) activeRacers.Add(go.transform);
        }
        foreach (GameObject go in aiCars)
        {
            if (go != null) activeRacers.Add(go.transform);
        }

        if (GameManager.Instance != null && GameManager.Instance.playerCarInstance != null)
        {
            if (!activeRacers.Contains(GameManager.Instance.playerCarInstance.transform))
            {
                activeRacers.Add(GameManager.Instance.playerCarInstance.transform);
            }
        }
    }



    void Update()
    {

        if (raceFinished) return;

        activeRacers.RemoveAll(racer => racer == null);

        activeRacers.Sort((a, b) => a.position.z.CompareTo(b.position.z));

        int playerPosition = -1;
        if (GameManager.Instance != null && GameManager.Instance.playerCarInstance != null)
        {
            playerPosition = activeRacers.FindIndex(r => r == GameManager.Instance.playerCarInstance.transform) + 1;
        }

        //if (positionText != null)
        //{
        //    if (playerPosition != -1)
        //    {
        //        positionText.text = "Position: " + playerPosition + " / " + activeRacers.Count;
        //    }
        //    else
        //    {
        //        positionText.text = "Position: N/A";
        //    }
        //}
        //positionText.text = "";
    }

    public void FinishRace(GameObject racer)
    {
        if (racer == null) return;

        if (!finishOrder.Any(fr => fr.racerName == racer.name))
        {
            float finalTime = timeCounter != null ? timeCounter.timeRemaining : 0f;

            finishOrder.Add(new FinishResult
            {
                racerName = racer.name,
                finishTime = finalTime
            });
        }

        Debug.Log(racer.name + " ukoñczy³ wyœcig!");

        // Jeœli gracz ukoñczy³ — zakoñcz wyœcig i wyœwietl wyniki
        if (racer.name.StartsWith("Player") || (GameManager.Instance != null && GameManager.Instance.playerCarInstance == racer))
        {
            Debug.Log("Gracz ukoñczy³ wyœcig");
            raceFinished = true;
            ShowResults();
        }
    }

    void ShowResults()
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }

        if (resultText != null)
        {
            resultText.text = "Results:\n";

            // Posortuj po kolejnoœci mety
            for (int i = 0; i < finishOrder.Count; i++)
            {
                var result = finishOrder[i];
                float finalTime = result.finishTime;
                int minutes = Mathf.FloorToInt(finalTime / 60f);
                int seconds = Mathf.FloorToInt(finalTime % 60f);
                string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);

                resultText.text += $"{i + 1}. {result.racerName} (Time: {formattedTime})\n";
            }
        }
    }

    public void RegisterRacer(Transform racer)
    {
        if (!activeRacers.Contains(racer))
        {
            activeRacers.Add(racer);
        }
    }

}
