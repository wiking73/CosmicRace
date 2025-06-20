using UnityEngine;
using TMPro;

public class TimeCounter : MonoBehaviour
{
    public float timeRemaining = 0f; 
    public TextMeshProUGUI timeText;
    public string textt = "Czas gry";

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
        timeText.text = textt + ": " + Mathf.Ceil(timeRemaining).ToString() + "s";

    }
}
