using UnityEngine;
using UnityEngine.SceneManagement; // Potrzebne do ładowania scen

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    private bool isPaused = false;

    void Start()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        Time.timeScale = 1f; 
        isPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
        Time.timeScale = 0f;
        isPaused = true;
        Debug.Log("Gra spauzowana.");
    }

    public void ResumeGame()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        Time.timeScale = 1f;
        isPaused = false;
        Debug.Log("Gra wznowiona.");
    }

    public void GoToMainMenu()
    {
        ResumeGame();
        SceneManager.LoadScene("mainMenu");
    }
}