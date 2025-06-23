using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject trackSelectPanel;
    public GameObject optionsPanel;
    public static string SelectedTrackSceneName = ""; 

    public void Play()
    {
        mainMenuPanel.SetActive(false);
        trackSelectPanel.SetActive(true);
    }

    public void Options()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void Exit()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    public void BackToMain()
    {
        trackSelectPanel.SetActive(false);
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void SelectTrackAndLoadVehicleSelectScene(string sceneNameOfTrack)
    {
        SelectedTrackSceneName = sceneNameOfTrack;
        SceneManager.LoadScene("VehicleSelectScene");
    }
}
