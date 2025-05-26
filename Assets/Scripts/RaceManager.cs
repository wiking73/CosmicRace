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

    private bool raceFinished = false;
    private List<string> finishOrder = new List<string>();

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (raceFinished) return;

        racers.Sort((a, b) => a.position.z.CompareTo(b.position.z)); // sortowanie po pozycji Z (na trasie od-do)

        int playerPosition = racers.FindIndex(r => r.CompareTag("Player")) + 1;
        positionText.text = "Pozycja: " + playerPosition + " / " + racers.Count;
    }

    public void FinishRace(string racerName)
    {
        if (!finishOrder.Contains(racerName))
        {
            finishOrder.Add(racerName);
        }

        if (racerName == "Player")
        {
            raceFinished = true;
            ShowResults();
        }
    }

    void ShowResults()
    {
        resultPanel.SetActive(true);
        resultText.text = "Wyniki:\n";

        for (int i = 0; i < finishOrder.Count; i++)
        {
            resultText.text += (i + 1) + ". " + finishOrder[i] + "\n";
            Debug.Log((i + 1) + ". " + finishOrder[i] + "\n");
        }

    }
}
