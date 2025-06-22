using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance;

    public List<Transform> racers = new List<Transform>();
    public Text positionText;
    public Text resultText;
    public GameObject resultPanel;
    public UnityEngine.UI.Text countdownText;
    public TimeCounter timeCounter;

    private bool raceFinished = false;
    private List<string> finishOrder = new List<string>();

    private void Awake()
    {
        Instance = this;
    }


    void Update()
    {

        if (raceFinished) return;

        racers.Sort((a, b) => a.position.z.CompareTo(b.position.z)); 

        int playerPosition = racers.FindIndex(r => r.name.StartsWith("Player")) + 1;
        positionText.text = "Position: " + playerPosition + " / " + racers.Count;
    }

    public void FinishRace(GameObject racer)
    {
        if (!finishOrder.Contains(racer.name))
        {
            finishOrder.Add(racer.name);
        }

        Debug.Log(racer.name);

        if (racer.name.StartsWith("Player"))
        {
            Debug.Log("Gracz ukonczyl wyscig");
            raceFinished = true;
            ShowResults();
        }
    }

    void ShowResults()
    {
        resultPanel.SetActive(true);
        resultText.text = "Results:\n";

        for (int i = 0; i < finishOrder.Count; i++)
        {
            string racerName = finishOrder[i];

            
            if (racerName.StartsWith("Player"))
            {
                float finalTime = timeCounter.timeRemaining;
                int minutes = Mathf.FloorToInt(finalTime / 60f);
                int seconds = Mathf.FloorToInt(finalTime % 60f);
                string formattedTime = string.Format("{0:00}:{1:00}", minutes, seconds);

                resultText.text += $"{i + 1}. {racerName} (Time: {formattedTime})\n";
            }
            else
            {
                resultText.text += $"{i + 1}. {racerName}\n";
            }
        }
    }

}
