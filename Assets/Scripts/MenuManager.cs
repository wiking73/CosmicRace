using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject trackSelectPanel;
    public GameObject optionsPanel;

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

    public void  LoadScene(int num)
    {
        SceneManager.LoadScene(num);
    }
}
