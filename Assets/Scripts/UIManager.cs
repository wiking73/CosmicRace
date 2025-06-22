using UnityEngine;
using TMPro; // Pamiętaj o tym, jeśli używasz TextMeshPro

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Prompts")]
    [SerializeField] private GameObject flipCarPromptGameObject;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (flipCarPromptGameObject != null)
        {
            flipCarPromptGameObject.SetActive(false);
            Debug.Log("UIManager: FlipCar UI Prompt hidden on Awake.");
        }
        else
        {
            Debug.LogWarning("UIManager: FlipCar Prompt Game Object is NOT assigned in the Inspector. Please assign it.", this);
        }
    }

    public void ShowFlipCarPrompt(bool show)
    {
        if (flipCarPromptGameObject != null)
        {
            flipCarPromptGameObject.SetActive(show);
        }
        else
        {
            Debug.LogWarning("UIManager: Attempted to show/hide FlipCar Prompt, but it's not assigned!", this);
        }
    }
}