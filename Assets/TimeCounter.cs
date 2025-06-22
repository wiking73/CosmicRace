using UnityEngine;
using TMPro;

public class TimeCounter : MonoBehaviour
{
    public float timeRemaining = 0f; 
    public TextMeshProUGUI timeText;
    public string textt = "Time";

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.raceStarted)
        {
            timeRemaining += Time.deltaTime;
            timeRemaining = Mathf.Max(0, timeRemaining);

            UpdateTimeDisplay();
        }
    }

    public void AddTime(float amount)
    {
        timeRemaining += amount;
        UpdateTimeDisplay();
    }

    void UpdateTimeDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timeText.text = $"{textt}: {minutes:00}:{seconds:00}";
    }

}
